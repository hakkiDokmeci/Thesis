# Graduate Thesis Database - ASP.NET Core API

Bu proje, yüksek lisans ve doktora tezlerini yönetmek için geliştirilmiş bir ASP.NET Core Web API uygulamasıdır.

## ?? Teknolojiler

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- C# 13.0

## ?? Özellikler

- **Tez Yönetimi**: Tez ekleme, güncelleme, listeleme ve silme
- **Kişi Yönetimi**: Yazarlar, danışmanlar ve diğer kişiler
- **Üniversite/Enstitü Yönetimi**: Akademik kurumlar
- **Konu ve Anahtar Kelime Yönetimi**: Tezlerin kategorize edilmesi
- **Çoklu Dil Desteği**: Farklı dillerdeki tezler
- **CORS Desteği**: Frontend uygulamaları için

## ??? Veritabanı Yapısı

Proje aşağıdaki ana tablolardan oluşur:
- `THESIS`: Tez bilgileri
- `PERSON`: Kişi bilgileri (Yazar, Danışman)
- `UNIVERSITY`: Üniversite bilgileri
- `INSTITUTE`: Enstitü bilgileri
- `SUBJECT`: Konu/Alan bilgileri
- `KEYWORD`: Anahtar kelimeler
- `LANGUAGE`: Dil bilgileri
- `THESIS_TYPE`: Tez tipleri (Yüksek Lisans, Doktora vb.)
- `THESIS_PERSON_ROLE`: Tez-Kişi ilişkileri
- `THESIS_SUBJECT`: Tez-Konu ilişkileri
- `THESIS_KEYWORD`: Tez-Anahtar Kelime ilişkileri

## ?? Kurulum

### Gereksinimler
- .NET 9.0 SDK
- SQL Server (LocalDB veya Express)
- Visual Studio 2022 veya Visual Studio Code

### Adımlar

1. **Projeyi klonlayın**
```bash
git clone https://github.com/YOUR_USERNAME/ThesisDb.git
cd ThesisDb
```

2. **Bağlantı dizesini yapılandırın**

`appsettings.json` dosyasında connection string'i kendi SQL Server bilgilerinize göre güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=GraduateThesisDB;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True"
  }
}
```

3. **Veritabanını oluşturun**

SQL Server'da veritabanı şemasını oluşturmak için SQL scriptlerini çalıştırın veya Entity Framework migrations kullanın:

```bash
dotnet ef database update
```

4. **Uygulamayı çalıştırın**
```bash
dotnet run
```

Uygulama varsayılan olarak `https://localhost:5001` ve `http://localhost:5000` adreslerinde çalışacaktır.

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

### Diğer Endpoints
- `/api/institutes` - Enstitüler
- `/api/people` - Kişiler
- `/api/subjects` - Konular
- `/api/keywords` - Anahtar kelimeler
- `/api/languages` - Diller
- `/api/thesistypes` - Tez tipleri

## ?? Örnek Kullanım

### Yeni Tez Ekleme

```json
POST /api/theses
{
  "title": "Yapay Zeka ve Makine Öğrenmesi Uygulamaları",
  "abstract": "Bu tez yapay zeka alanında...",
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
  "keywords": ["yapay zeka", "makine öğrenmesi", "derin öğrenme"]
}
```

## ?? Katkıda Bulunma

1. Bu projeyi fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## ?? Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## ?? İletişim

Proje Sahibi - [@Brkays](https://github.com/YOUR_GITHUB_USERNAME)

Proje Linki: [https://github.com/YOUR_GITHUB_USERNAME/ThesisDb](https://github.com/YOUR_GITHUB_USERNAME/ThesisDb)
