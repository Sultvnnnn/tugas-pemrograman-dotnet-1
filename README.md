# IF133 - Pertemuan 4 — Tugas 1 Final: EF Core & Data Persistence

## Identitas
- Nama: Sultan Abdul Fatah
- NIM: 1003240033
- Kelas: Informatika Eksekutif

## Studi Kasus
**Produk / Inventory** dengan entity `Product`. Entity memilih opsi A pada soal: mengelola data produk sederhana (nama, deskripsi, harga, stok) dengan operasi CRUD penuh melalui REST API.

Field `Product`:
| Field | Tipe | Wajib | Aturan |
|---|---|---|---|
| Id | int | Ya | Primary key, generated |
| Name | string | Ya | Tidak kosong, max 200 karakter |
| Description | string? | Tidak | Max 1000 karakter |
| Price | decimal | Ya | >= 0 |
| Stock | int | Ya | >= 0 |

## Teknologi
- ASP.NET Core Web API (.NET 10, controller-based)
- Entity Framework Core 10
- Database provider: SQLite (`app.db`)

## Struktur Project
```
IF133_P4_1003240033_SultanAbdulFatah/
├── IF133.Tugas1.slnx
├── IF133.Tugas1.Api/
│   ├── Controllers/
│   │   └── ProductsController.cs   # Endpoint CRUD
│   ├── Data/
│   │   └── AppDbContext.cs          # DbContext + DbSet<Product> + HasData seed
│   ├── Entities/
│   │   └── Product.cs               # Entity + Data Annotations validasi
│   ├── Migrations/                  # InitialCreate + SeedProducts
│   ├── Properties/launchSettings.json
│   ├── Program.cs                   # Konfigurasi & DI
│   ├── appsettings.json             # Connection string
│   ├── app.db                       # Database SQLite hasil migration
│   └── IF133.Tugas1.Api.csproj
├── P4_Postman_Collection.json      # Collection pengujian (ada assertion)
├── IF133_Tugas1_Postman_Collection.json  # Collection tugas sebelumnya
├── README.md
├── screenshots/                     # Bukti pengujian Pertemuan 4
└── screenshots_tugas1/              # Arsip bukti Pertemuan 3
```

Pemisahan tanggung jawab:
- **Entities/** — model data + aturan validasi (Data Annotations).
- **Data/** — `AppDbContext`, jembatan entity ke database.
- **Controllers/** — endpoint HTTP, tidak berisi connection string.
- **Program.cs** — registrasi `AppDbContext` melalui dependency injection.

## Cara Menjalankan
1. Restore dependency:
   ```
   dotnet restore
   ```
2. Update database (membuat `app.db` dari migration):
   ```
   dotnet ef database update --project IF133.Tugas1.Api
   ```
3. Run project:
   ```
   dotnet run --project IF133.Tugas1.Api
   ```
4. Buka Swagger UI: `http://localhost:5154/swagger` (development) — atau uji via Postman memakai `P4_Postman_Collection.json`.

## Konfigurasi Penting
**appsettings.json** (connection string, bukan di controller):
```json
"ConnectionStrings": { "DefaultConnection": "Data Source=app.db" }
```

**Program.cs** — registrasi DbContext:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## Migration
Command yang dijalankan:
```
dotnet ef migrations add InitialCreate --project IF133.Tugas1.Api
dotnet ef migrations add SeedProducts --project IF133.Tugas1.Api
dotnet ef database update --project IF133.Tugas1.Api
```

Output `dotnet ef database update --project IF133.Tugas1.Api` pada database baru menunjukkan kedua migration diterapkan dan tiga data dimasukkan:
```text
Applying migration '20261020082229_InitialCreate'.
Applying migration '20261026070406_SeedProducts'.
INSERT INTO "Products" ("Id", "Description", "Name", "Price", "Stock")
VALUES (1, 'Keyboard 75% hot-swap', 'Keyboard Mechanical', '350000.0', 5);
INSERT INTO "Products" ("Id", "Description", "Name", "Price", "Stock")
VALUES (2, 'Mouse wireless 2.4GHz', 'Mouse Gaming', '250000.0', 10);
INSERT INTO "Products" ("Id", "Description", "Name", "Price", "Stock")
VALUES (3, 'Monitor IPS 75Hz', 'Monitor 24 Inch', '1500000.0', 3);
Done.
```

Pemeriksaan dari folder `IF133.Tugas1.Api`:
```console
$ sqlite3 app.db ".tables"
Products               __EFMigrationsHistory  __EFMigrationsLock

$ sqlite3 app.db ".schema Products"
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Price" TEXT NOT NULL,
    "Stock" INTEGER NOT NULL
);

$ sqlite3 app.db "SELECT * FROM Products;"
1|Keyboard Mechanical|Keyboard 75% hot-swap|350000.0|5
2|Mouse Gaming|Mouse wireless 2.4GHz|250000.0|10
3|Monitor 24 Inch|Monitor IPS 75Hz|1500000.0|3

$ sqlite3 app.db "SELECT MigrationId FROM __EFMigrationsHistory;"
20261020082229_InitialCreate
20261026070406_SeedProducts
```

## Endpoint
| Operasi | Method | Endpoint | Success | Error |
|---|---|---|---|---|
| Get all | GET | /api/products | 200 | 500 |
| Get by id | GET | /api/products/{id} | 200 | 404 |
| Create | POST | /api/products | 201 | 400 |
| Update | PUT | /api/products/{id} | 200 | 400 / 404 |
| Delete | DELETE | /api/products/{id} | 204 | 404 |

Contoh POST:
```
POST /api/products
Content-Type: application/json

{"name": "Keyboard Mechanical", "description": "Keyboard 75%", "price": 350000, "stock": 5}
```

## Data Contoh (Seed)
Tiga data contoh ditambahkan melalui `HasData` pada `OnModelCreating` di `AppDbContext` dan masuk ke database melalui migration `SeedProducts`:

| Id | Name | Description | Price | Stock |
|---|---|---|---|---|
| 1 | Keyboard Mechanical | Keyboard 75% hot-swap | 350000 | 5 |
| 2 | Mouse Gaming | Mouse wireless 2.4GHz | 250000 | 10 |
| 3 | Monitor 24 Inch | Monitor IPS 75Hz | 1500000 | 3 |

## Bukti Pengujian

Pengujian dilakukan di **Postman** dengan collection `P4_Postman_Collection.json` (8 request, tiap request punya test script/assertion otomatis). Hasil akhir: **8 request berhasil, 23 assertion lulus, 0 gagal**. Screenshot tersimpan di folder `screenshots/` (bukti Pertemuan 3 diarsipkan di `screenshots_tugas1/`).

| No | Skenario | Request | Status | Hasil | Screenshot |
|---|---|---|---|---|---|
| 1 | GET seluruh data | GET /api/products | 200 | Array 3 data seed (Keyboard Mechanical, Mouse Gaming, Monitor 24 Inch); assertion 3/3 | `1_get_all_seed.png` |
| 2 | GET by ID valid | GET /api/products/1 | 200 | `{"id":1,"name":"Keyboard Mechanical","description":"Keyboard 75% hot-swap","price":350000.0,"stock":5}`; assertion 3/3 | `2_get_by_id.png` |
| 3 | POST data valid | POST /api/products `{"name":"Webcam Full HD","description":"Webcam 1080p dengan mikrofon","price":450000,"stock":8}` | 201 | Data baru `id: 4`; assertion 3/3 | `3_post_201.png` |
| 4 | GET hasil POST (bukti persistence) | GET /api/products/4 | 200 | Data tersimpan di database dan terbaca kembali; assertion 3/3 | `4_get_after_post_persistence.png` |
| 5 | PUT update data | PUT /api/products/4 `{"name":"Webcam Full HD Pro","description":"Webcam 1080p + ring light","price":550000,"stock":12}` | 200 | Data terupdate; assertion 3/3 | `5_put_update.png` |
| 6 | DELETE data | DELETE /api/products/4 | 204 | Response body kosong (No Content); assertion 2/2 | `6_delete_204.png` |
| 7 | GET ID tidak ada | GET /api/products/999 | 404 | ProblemDetails `{"title":"Not Found","status":404}` | `7_get_404.png` |
| 8 | POST data tidak valid | POST /api/products `{"name":"","description":"Data tidak valid","price":-5000,"stock":-1}` | 400 | Error per field: `Name` "Name is required" + "Name cannot be empty", `Price` "Price must be >= 0", `Stock` "Stock must be >= 0" | `8_post_400_validasi.png` |

Skenario 7 dan 8 adalah pengujian gagal yang diwajibkan soal. Skenario 3 → 4 membuktikan data benar-benar tersimpan (persistence), bukan hanya dikembalikan di response.

### Bukti Migration dan Database Update
Bukti terdapat pada bagian **Migration**: perintah `migrations add InitialCreate`, `migrations add SeedProducts`, dan `database update`, diikuti output penerapan migration, `.tables`, `.schema Products`, isi `Products`, serta isi `__EFMigrationsHistory`.

Screenshot bukti di folder `screenshots/`:

| File | Isi |
|---|---|
| `9_migrations_list.png` | `dotnet ef migrations list` — dua migration terdaftar |
| `13_database_update_apply_1.png`, `13_database_update_apply_2.png` | `dotnet ef database update` — kedua migration diaplikasikan, `CREATE TABLE "Products"`, tiga `INSERT` data seed, `Done.` |
| `10_skema_tabel_products.png` | `sqlite3 app.db ".schema Products"` — struktur tabel hasil migration |
| `11_data_seed_products.png` | `sqlite3 app.db "SELECT * FROM Products;"` — tiga data seed |
| `12_riwayat_migration.png` | `sqlite3 app.db "SELECT * FROM __EFMigrationsHistory;"` — dua migration tercatat di database |

`bin/` dan `obj/` tidak disertakan dalam folder pengumpulan karena merupakan hasil build. Jalankan `dotnet restore` lalu `dotnet run --project IF133.Tugas1.Api` untuk membuatnya kembali. File `app.db` disertakan sebagai database hasil migration (berisi tiga data seed); bila ingin membuat ulang dari nol, hapus `app.db` lalu jalankan `dotnet ef database update --project IF133.Tugas1.Api`.

## Kendala dan Solusi
1. **`dotnet-ef` belum terinstal** — command `dotnet ef` tidak dikenali karena tool global EF belum ada. Solusi: `dotnet tool install --global dotnet-ef`.
2. **Template bawaan berisi WeatherForecast** — controller dan model contoh harus dihapus agar tidak membingungkan struktur. Solusi: hapus `WeatherForecast.cs` dan `WeatherForecastController.cs`, ganti dengan entity/DbContext/controller studi kasus sendiri.
3. **Template .NET 10 tidak menyediakan Swagger UI** — template `dotnet new webapi` hanya mendaftarkan `AddOpenApi()`/`MapOpenApi()` sehingga `/swagger` mengembalikan 404. Solusi: tambahkan package `Swashbuckle.AspNetCore`, lalu `AddSwaggerGen()` pada service dan `UseSwagger()` + `UseSwaggerUI()` pada pipeline development.
4. **`sqlite3` hanya membuat file kosong jika database belum ada** — menjalankan query sebelum `dotnet ef database update` menghasilkan error `no such table: Products`. Solusi: terapkan migration lebih dahulu, baru lakukan pemeriksaan tabel.
5. **Data contoh harus ikut tercatat sebagai migration** — menambahkan data lewat kode runtime tidak terlihat di bukti migration. Solusi: gunakan `HasData` pada `OnModelCreating` sehingga data ikut dalam migration `SeedProducts`.

## Kesimpulan
Skeleton Web API + persistence EF Core berjalan penuh: solution terstruktur, DbContext terdaftar melalui dependency injection, connection string berada di konfigurasi aplikasi, dua migration (`InitialCreate` dan `SeedProducts`) berhasil dibuat dan diterapkan, tabel `Products` terbentuk, tiga data contoh tersimpan dan terbaca kembali oleh aplikasi, CRUD lengkap dengan status code yang tepat, validasi berjalan, serta seluruh 8 skenario pengujian Postman lolos (23 assertion) termasuk skenario gagal 404 dan 400.
