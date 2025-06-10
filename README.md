
# 🏭 Factory360

Factory360 is a fully-featured production management system developed to digitize and improve the efficiency of operations in small and medium-sized factories. Built with ASP.NET Core MVC and Entity Framework, this system offers a sleek interface that allows you to easily manage worker check-ins and check-outs, employee records, role assignments and deletions, salary and hourly wage settings, wage calculations based on active working hours, activity reports, and admin operations.

---

Factory360, küçük ve orta ölçekli fabrikaların operasyonlarını dijitalleştirmek ve daha verimli hale getirmek amacıyla geliştirilmiş tam donanımlı bir üretim yönetim sistemidir. ASP.NET Core MVC ve Entity Framework kullanılarak geliştirilen bu sistem, işçi giriş-çıkışlarını, işçi kayıtlarını, rol atamayı, rol silmeyi, maaş ve saatlik ücret belirlemeyi, işçinin aktif olduğu saatlerde ücret hesaplaması yapabilmeyi, aktiflik rapolarını ve admin işlemlerini  kolayca yönetebileceğiniz şık bir arayüz sunar.

## 🚀 Features / Özellikler
 
- Employee tracking and management   
- Employee Check-in and Check-out Reports
- Real-time data updates via Entity Framework  
- Clean Razor-based UI  
- SQLite database support  
- Modular MVC architecture

---


- Çalışan yönetimi
- İşçi giriş ve çıkış raporları
- Gerçek zamanlı veri güncellemeleri (Entity Framework)  
- Temiz ve düzenli Razor tabanlı arayüz  
- SQLite veritabanı desteği  
- Modüler MVC mimarisi

## ⚙️ Installation & Usage / Kurulum ve Kullanım

### Prerequisites / Gereksinimler

- .NET 9.0 SDK  
- Visual Studio 2022 veya VS Code

### Getting Started

```bash
# Clone the repository
git clone https://github.com/Sandoval-dev/Factory360.git
cd Factory360

# Run the project
dotnet run
```

> Note: The system uses a local SQLite database file (`Factory.db`). Make sure the connection string in `appsettings.json` is correct for your environment.

> Not: Sistem yerel bir SQLite veritabanı dosyası (`Factory.db`) kullanır. `appsettings.json` dosyasındaki bağlantı dizesinin ortamınıza uygun olduğundan emin olun.

## 📁 Project Structure / Proje Yapısı

```
Factory360/
├── Controllers/      # Application logic
├── Models/           # Entity models
├── Views/            # Razor pages
├── Data/             # DB context
├── Migrations/       # EF Core migrations
├── wwwroot/          # Static assets
├── Program.cs        # App entry
└── appsettings.json  # Configuration
```

## 📄 License / Lisans

MIT License © 2025 Sandoval-dev
