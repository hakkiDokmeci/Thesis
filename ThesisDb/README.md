# Graduate Thesis Database - ASP.NET Core API

Bu proje, yüksek lisans ve doktora tezlerini yönetmek için geliþtirilmiþ bir ASP.NET Core Web API uygulamasýdýr.

## ?? Teknolojiler

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- C# 13.0

## ?? Özellikler

- **Tez Yönetimi**: Tez ekleme, güncelleme, listeleme ve silme
- **Kiþi Yönetimi**: Yazarlar, danýþmanlar ve diðer kiþiler
- **Üniversite/Enstitü Yönetimi**: Akademik kurumlar
- **Konu ve Anahtar Kelime Yönetimi**: Tezlerin kategorize edilmesi
- **Çoklu Dil Desteði**: Farklý dillerdeki tezler
- **CORS Desteði**: Frontend uygulamalarý için

## ??? Veritabaný Yapýsý

Proje aþaðýdaki ana tablolardan oluþur:
- `THESIS`: Tez bilgileri
- `PERSON`: Kiþi bilgileri (Yazar, Danýþman)
- `UNIVERSITY`: Üniversite bilgileri
- `INSTITUTE`: Enstitü bilgileri
- `SUBJECT`: Konu/Alan bilgileri
- `KEYWORD`: Anahtar kelimeler
- `LANGUAGE`: Dil bilgileri
- `THESIS_TYPE`: Tez tipleri (Yüksek Lisans, Doktora vb.)
- `THESIS_PERSON_ROLE`: Tez-Kiþi iliþkileri
- `THESIS_SUBJECT`: Tez-Konu iliþkileri
- `THESIS_KEYWORD`: Tez-Anahtar Kelime iliþkileri

## ?? Kurulum

### Gereksinimler
- .NET 9.0 SDK
- SQL Server (LocalDB veya Express)
- Visual Studio 2022 veya Visual Studio Code

### Adýmlar

1. **Projeyi klonlayýn**
```bash
git clone https://github.com/YOUR_USERNAME/ThesisDb.git
cd ThesisDb
```

2. **Baðlantý dizesini yapýlandýrýn**

`appsettings.json` dosyasýnda connection string'i kendi SQL Server bilgilerinize göre güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=GraduateThesisDB;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True"
  }
}
```

3. **Veritabanýný oluþturun**

SQL Server'da veritabaný þemasýný oluþturmak için SQL scriptlerini çalýþtýrýn veya Entity Framework migrations kullanýn:

```bash
dotnet ef database update
```

4. **Uygulamayý çalýþtýrýn**
```bash
dotnet run
```

Uygulama varsayýlan olarak `https://localhost:5001` ve `http://localhost:5000` adreslerinde çalýþacaktýr.

## ?? API Endpoints

### Tezler (Theses)
- `GET /api/theses` - Tüm tezleri listele
- `GET /api/theses/{id}` - Belirli bir tezi getir
- `POST /api/theses` - Yeni tez ekle
- `PUT /api/theses/{id}` - Tez güncelle
- `DELETE /api/theses/{id}` - Tez sil

### Üniversiteler (Universities)
- `GET /api/universities` - Tüm üniversiteleri listele
- `GET /api/universities/{id}` - Belirli bir üniversiteyi getir
- `POST /api/universities` - Yeni üniversite ekle
- `PUT /api/universities/{id}` - Üniversite güncelle
- `DELETE /api/universities/{id}` - Üniversite sil

### Diðer Endpoints
- `/api/institutes` - Enstitüler
- `/api/people` - Kiþiler
- `/api/subjects` - Konular
- `/api/keywords` - Anahtar kelimeler
- `/api/languages` - Diller
- `/api/thesistypes` - Tez tipleri

## ?? Örnek Kullaným

### Yeni Tez Ekleme

```json
POST /api/theses
{
  "title": "Yapay Zeka ve Makine Öðrenmesi Uygulamalarý",
  "abstract": "Bu tez yapay zeka alanýnda...",
  "year": 2024,
  "numberOfPages": 150,
  "submissionDate": "2024-06-15",
  "typeId": 1,
  "languageId": 1,
  "instituteId": 1,
  "authorId": 1,
  "supervisorIds": [2, 3],
  "coSupervisorId": 4,
  "subjectIds": [1, 2],
  "keywords": ["yapay zeka", "makine öðrenmesi", "derin öðrenme"]
}
```

## ?? Katkýda Bulunma

1. Bu projeyi fork edin
2. Feature branch oluþturun (`git checkout -b feature/AmazingFeature`)
3. Deðiþikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluþturun

## ?? Lisans

Bu proje MIT lisansý altýnda lisanslanmýþtýr.

## ?? Ýletiþim

Proje Sahibi - [@YOUR_GITHUB_USERNAME](https://github.com/YOUR_GITHUB_USERNAME)

Proje Linki: [https://github.com/YOUR_GITHUB_USERNAME/ThesisDb](https://github.com/YOUR_GITHUB_USERNAME/ThesisDb)
