# IF133 - Tugas 1

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
IF133-Tugas1/
├── IF133.Tugas1.sln
└── IF133.Tugas1.Api/
    ├── Controllers/
    │   └── ProductsController.cs   # Endpoint CRUD
    ├── Data/
    │   └── AppDbContext.cs          # DbContext + DbSet<Product>
    ├── Entities/
    │   └── Product.cs               # Entity + Data Annotations validasi
    ├── Migrations/                  # Hasil dotnet ef migrations add
    ├── Properties/launchSettings.json
    ├── Program.cs                   # Konfigurasi & DI
    ├── appsettings.json             # Connection string
    └── IF133.Tugas1.Api.csproj
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
4. Buka Swagger/OpenAPI: `http://localhost:5154/openapi/v1.json` (dev) — atau uji via Swagger UI / Postman.

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
dotnet ef database update --project IF133.Tugas1.Api
```
Hasil: migration `InitialCreate` berhasil dibuat dan diaplikasikan. Tabel `Products` terbentuk di `app.db`:
```sql
CREATE TABLE "Products" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Price" TEXT NOT NULL,
    "Stock" INTEGER NOT NULL
);
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

## Bukti Pengujian
Skenario diuji via **Postman** menggunakan collection `IF133_Tugas1_Postman_Collection.json` (tersedia di folder pengumpulan). Screenshot tiap skenario tersimpan di folder `screenshots/` (termasuk `8_struktur_project_vscode.png` — struktur solution/project di VS Code).

| No | Skenario | Request | Status | Hasil |
|---|---|---|---|---|
| 1 | GET semua data | GET /api/products | 200 | Array berisi data produk |
| 2 | POST data valid | POST /api/products, body `{"name": "Mouse Gaming", "description": "Wireless 2.4GHz", "price": 250000, "stock": 10}` | 201 | Data baru terbentuk: `{"id": 3, "name": "Mouse Gaming", ...}` |
| 3 | GET by ID valid | GET /api/products/2 | 200 | Satu object: `{"id": 2, "name": "Keyboard Mechanical", "description": "Keyboard 75%", "price": 350000.0, "stock": 5}` |
| 4 | PUT update data | PUT /api/products/2, body `{"name": "Mouse Gaming Pro", "description": "Wireless 8K Hz", "price": 350000, "stock": 7}` | 200 | Data terupdate: `{"id": 2, "name": "Mouse Gaming Pro", "price": 350000, "stock": 7}` |
| 5 | DELETE by ID | DELETE /api/products/2 | 204 | Response body kosong (No Content) |
| 6 | GET ID tidak ada | GET /api/products/999 | 404 | ProblemDetails: `{"title": "Not Found", "status": 404}` |
| 7 | POST data tidak valid | POST /api/products, body `{"name": "", "description": "", "price": -5000, "stock": -1}` | 400 | Error per field: `Name`: "Name is required", "Name cannot be empty"; `Price`: "Price must be >= 0" |

Skenario 6 & 7 adalah pengujian gagal yang wajib didokumentasikan.

Catatan validasi pada skenario 7: field `stock` bernilai negatif juga tercakup aturan `[Range(0, ...)]` pada entity; response menampilkan pesan error untuk setiap field yang melanggar.

## Kendala dan Solusi
1. **`dotnet-ef` belum terinstal** — command `dotnet ef` tidak dikenali karena tool global EF belum ada. Solusi: `dotnet tool install --global dotnet-ef`.
2. **Template bawaan berisi WeatherForecast** — controller dan model contoh harus dihapus agar tidak membingungkan struktur. Solusi: hapus `WeatherForecast.cs` dan `WeatherForecastController.cs`, ganti dengan entity/DbContext/controller studi kasus sendiri.

## Kesimpulan
Skeleton Web API + persistence EF Core berjalan penuh: solution terstruktur, migration berhasil, CRUD lengkap dengan status code yang tepat, validasi berjalan, dan seluruh 7 skenario pengujian lolos termasuk skenario gagal (404 dan 400).
