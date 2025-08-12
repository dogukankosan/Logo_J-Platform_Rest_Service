
# 🔗 Logo J-Platform REST Service

<img width="1660" height="785" alt="SS" src="https://github.com/user-attachments/assets/9cae7ac4-4cc7-49d6-9511-7843000bb56c" />
![License](https://img.shields.io/github/license/dogukankosan/Logo_J-Platform_Rest_Service)
![Stars](https://img.shields.io/github/stars/dogukankosan/Logo_J-Platform_Rest_Service)
![Issues](https://img.shields.io/github/issues/dogukankosan/Logo_J-Platform_Rest_Service)
![Last Commit](https://img.shields.io/github/last-commit/dogukankosan/Logo_J-Platform_Rest_Service)

> **Logo J-Platform REST Service**, Logo J-Platform REST API ile entegre çalışan, token tabanlı oturum yönetimi, cari kart işlemleri, muhasebe fişi (GL Slip) yönetimi ve Excel dosya okuma özelliklerini destekleyen kapsamlı bir C#/.NET WinForms uygulamasıdır.

---

## 🚀 Temel Özellikler

### 🔐 Oturum Yönetimi
- Token tabanlı güvenli oturum başlatma ve sonlandırma
- Otomatik token yenileme ve süre yönetimi
- SQLite ile güvenli kimlik bilgisi saklama

### 🧾 Cari Kart İşlemleri
- Yeni cari kart oluşturma (POST /v2.0/arps)
- Mevcut cari kartları güncelleme
- Aktif/Pasif durum değiştirme (PUT /v2.0/arps/status)
- Cari kart listeleme ve filtreleme

### 📊 Muhasebe Fişi (GL Slip) Modülü
- Genel muhasebe fişi oluşturma ve yönetimi
- Borç-Alacak kayıtları ile tam muhasebe entegrasyonu
- Fiş onay durumu takibi
- Otomatik fiş numaralandırma

### 📋 Excel Entegrasyonu
- Excel dosyalarından toplu cari kart aktarımı
- Excel formatında muhasebe fişi şablonları
- Veri doğrulama ve hata raporlama
- Desteklenen formatlar: .xlsx, .xls

### 💻 Kullanıcı Arayüzü
- DevExpress kontrolleri ile modern ve kullanıcı dostu arayüz
- Responsive tasarım ve kolay navigasyon
- Gerçek zamanlı durum bildirimleri
- Çoklu dil desteği (Türkçe/İngilizce)

---

## 🗂 Proje Yapısı

```
Logo_J-Platform_Rest_Service/
├── Forms/
│   ├── ArpCreateForm.cs         # Cari oluşturma ekranı
│   ├── ArpListForm.cs           # Cari listeleme ekranı
│   ├── GLSlipForm.cs            # Muhasebe fişi ekranı
│   └── ExcelImportForm.cs       # Excel aktarım ekranı
├── Models/
│   ├── Arp.cs                   # Cari model sınıfı
│   ├── SessionResult.cs         # Oturum sonuç modeli
│   ├── GLSlip.cs                # Muhasebe fişi modeli
│   └── ExcelDataModel.cs        # Excel veri modeli
├── Helpers/
│   ├── JPlatformSessionManager.cs  # Login/Logout yönetimi
│   ├── ArpValidator.cs             # Cari alan doğrulama
│   ├── GLSlipManager.cs            # Muhasebe fişi yönetimi
│   └── ExcelReader.cs              # Excel okuma servisi
├── Services/
│   ├── RestApiService.cs           # REST API istemci servisi
│   └── DataValidationService.cs    # Veri doğrulama servisi
├── SQLite/
│   └── RestSettings.db             # Ayarlar ve önbellek
├── Resources/
│   ├── Templates/                  # Excel şablonları
│   └── Icons/                      # Uygulama ikonları
├── Program.cs                      # Uygulama giriş noktası
└── README.md                       # Bu dokümantasyon
```

---

## 🛠️ Kurulum ve Başlangıç

### Ön Gereksinimler
- .NET Framework 4.7.2 veya üzeri
- Visual Studio 2019 veya üzeri
- Logo J-Platform erişim izni
- Microsoft Excel (Excel işlemleri için)

### Kurulum Adımları

1. **Projeyi İndirin**
   ```bash
   git clone https://github.com/dogukankosan/Logo_J-Platform_Rest_Service.git
   cd Logo_J-Platform_Rest_Service
   ```

2. **Proje Bağımlılıklarını Yükleyin**
   - NuGet paketleri otomatik olarak geri yüklenecektir
   - DevExpress lisansınızın geçerli olduğundan emin olun

3. **İlk Konfigürasyon**
   - Uygulamayı başlattığınızda ayarlar ekranı açılacaktır
   - Logo J-Platform bağlantı bilgilerinizi girin:
     - Server URL
     - Kullanıcı adı
     - Şifre
     - Database bilgileri

4. **Uygulamayı Başlatın**
   - Visual Studio'da F5 ile debug modunda çalıştırın
   - Veya Release build alıp exe dosyasını çalıştırın

---

## ⚙️ Kullanım Kılavuzu

### 🔑 Oturum Yönetimi
1. Uygulama başlatıldığında otomatik login işlemi yapılır
2. Token geçersiz ise yeniden authentication gerçekleştirilir
3. Uygulama kapatılırken güvenli logout işlemi yapılır

### 👥 Cari Kart İşlemleri
1. **Yeni Cari Oluşturma:**
   - "Yeni Cari" butonuna tıklayın
   - Gerekli alanları doldurun (kod, unvan, adres vb.)
   - "Kaydet" ile Logo sistemine gönderilir

2. **Cari Güncelleme:**
   - Cari listesinden güncellenecek kaydı seçin
   - Değişiklikleri yapın ve "Güncelle" butonuna tıklayın

3. **Durum Değiştirme:**
   - Cariyi seçin ve "Aktif/Pasif" butonunu kullanın

### 📊 Muhasebe Fişi (GL Slip) İşlemleri
1. **Yeni Fiş Oluşturma:**
   - "Muhasebe Fişi" menüsünden "Yeni Fiş" seçin
   - Fiş tarihini ve açıklamasını girin
   - Borç ve alacak kayıtlarını ekleyin
   - Toplam kontrolü yapıldıktan sonra kaydedin

2. **Fiş Onaylama:**
   - Oluşturulan fişler onay bekleyen durumda görünür
   - Fiş detaylarını kontrol edip onayla

### 📋 Excel İle Toplu İşlemler
1. **Excel'den Cari Aktarımı:**
   - "Excel Aktarım" menüsünden "Cari Aktarımı" seçin
   - Excel dosyanızı seçin (.xlsx veya .xls)
   - Veri eşleştirmesi yapın (Excel sütunları → Logo alanları)
   - Önizleme yapıp toplu aktarımı başlatın

2. **Desteklenen Excel Formatı:**
   ```
   | Cari Kodu | Ünvan | Telefon | E-posta | Adres |
   |-----------|-------|---------|---------|-------|
   | C001      | ABC Ltd | 555-0001 | abc@mail.com | İstanbul |
   ```

---

## 📡 API Endpoint Referansı

| HTTP Method | Endpoint | Açıklama | Örnek Kullanım |
|-------------|----------|----------|----------------|
| `POST` | `/auth/login` | Oturum başlatma | Login işlemi |
| `POST` | `/v2.0/arps` | Cari kart oluşturma | Yeni müşteri ekleme |
| `PUT` | `/v2.0/arps/{id}` | Cari kart güncelleme | Müşteri bilgisi değiştirme |
| `PUT` | `/v2.0/arps/status` | Durum değiştirme (arpCode) | Müşteriyi pasife alma |
| `PUT` | `/v2.0/arps/status/ref` | LogicalRef ile durum güncelleme | Ref ile aktif/pasif |
| `POST` | `/v2.0/glslips` | Muhasebe fişi oluşturma | Genel muhasebe kaydı |
| `GET` | `/v2.0/glslips` | Muhasebe fişi listeleme | Fiş sorgulama |
| `PUT` | `/v2.0/glslips/{id}/approve` | Fiş onaylama | Muhasebe fişi onayı |
| `POST` | `/auth/logout` | Oturum sonlandırma | Güvenli çıkış |

---

## 🔧 Teknik Detaylar

### Kullanılan Teknolojiler
- **Framework:** .NET Framework 4.7.2
- **UI Library:** DevExpress WinForms 23.x
- **Database:** SQLite (ayarlar ve önbellek)
- **HTTP Client:** HttpClient ile asenkron işlemler
- **Excel Processing:** EPPlus Library
- **JSON Serialization:** Newtonsoft.Json
- **Logging:** NLog

### Sistem Gereksinimleri
- **İşletim Sistemi:** Windows 10/11, Windows Server 2016+
- **RAM:** Minimum 4GB (8GB önerili)
- **Disk Alanı:** 500MB
- **Network:** İnternet bağlantısı (Logo J-Platform erişimi)

---

## 🐛 Bilinen Sorunlar ve Çözümleri

### Sık Karşılaşılan Hatalar

1. **Token Süresi Dolması**
   - **Hata:** "Unauthorized 401"
   - **Çözüm:** Uygulama otomatik token yeniler, sorun devam ederse yeniden giriş yapın

2. **Excel Dosya Formatı Hatası**
   - **Hata:** "Dosya okunamadı"
   - **Çözüm:** Excel dosyasının .xlsx veya .xls formatında olduğundan emin olun

3. **Bağlantı Zaman Aşımı**
   - **Hata:** "Connection Timeout"
   - **Çözüm:** Logo J-Platform server'ının erişilebilir olduğunu kontrol edin

---


## 🤝 Katkıda Bulunma

Bu projeyi geliştirmek için katkılarınızı bekliyoruz! 

### Katkı Süreci
1. Projeyi fork edin
2. Yeni bir feature branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik: XYZ eklendi'`)
4. Branch'inizi push edin (`git push origin feature/yeni-ozellik`)
5. Pull Request oluşturun

### Raporlama
- Hata bildirimleri ve öneriler için [Issues sekmesi](https://github.com/dogukankosan/Logo_J-Platform_Rest_Service/issues)ni kullanın
- Detaylı hata raporu yazın ve mümkünse ekran görüntüsü ekleyin

---

## 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

```
MIT License

Copyright (c) 2024 Doğukan Kosan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files...
```

## 🏷️ Versiyon Geçmişi

### v2.1.0 (Güncel)
- ✅ Muhasebe fişi (GL Slip) modülü eklendi
- ✅ Excel aktarım özelliği eklendi
- ✅ Gelişmiş hata yönetimi
- ✅ UI/UX iyileştirmeleri

### v2.0.0
- ✅ DevExpress UI entegrasyonu
- ✅ Token tabanlı authentication
- ✅ SQLite database entegrasyonu

### v1.0.0
- ✅ Temel cari kart işlemleri
- ✅ REST API entegrasyonu

---

<div align="center">

### ⚡ Hızlı Başlangıç

```bash
git clone https://github.com/dogukankosan/Logo_J-Platform_Rest_Service.git
cd Logo_J-Platform_Rest_Service
# Visual Studio ile açın ve F5 ile çalıştırın
```

**Made with ❤️ by [Doğukan Kosan](https://github.com/dogukankosan)**

</div>

---

<p align="center">
  <img src="https://img.shields.io/badge/.NET_Framework-4.7.2+-blue?logo=dotnet&logoColor=white" alt=".NET Framework" />
  <img src="https://img.shields.io/badge/DevExpress-WinForms-orange?logo=devexpress&logoColor=white" alt="DevExpress" />
  <img src="https://img.shields.io/badge/SQLite-Database-lightgrey?logo=sqlite&logoColor=white" alt="SQLite" />
  <img src="https://img.shields.io/badge/Excel-Integration-green?logo=microsoftexcel&logoColor=white" alt="Excel" />
  <img src="https://img.shields.io/badge/REST-API-red?logo=fastapi&logoColor=white" alt="REST API" />
</p>
