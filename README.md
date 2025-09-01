# 🔗 Logo J-Platform REST Service

<img width="1773" height="789" alt="pic" src="https://github.com/user-attachments/assets/555b42a3-81f1-41c6-9543-05c4b95a1002" />

[![License](https://img.shields.io/github/license/dogukankosan/Logo_J-Platform_Rest_Service)](LICENSE) 
[![Stars](https://img.shields.io/github/stars/dogukankosan/Logo_J-Platform_Rest_Service)](https://github.com/dogukankosan/Logo_J-Platform_Rest_Service/stargazers) 
[![Issues](https://img.shields.io/github/issues/dogukankosan/Logo_J-Platform_Rest_Service)](https://github.com/dogukankosan/Logo_J-Platform_Rest_Service/issues) 
[![Last Commit](https://img.shields.io/github/last-commit/dogukankosan/Logo_J-Platform_Rest_Service)](https://github.com/dogukankosan/Logo_J-Platform_Rest_Service/commits/main) 
[![.NET Framework](https://img.shields.io/badge/.NET-Framework_4.8-blue?logo=dotnet)](https://learn.microsoft.com/dotnet/) 
[![Windows Forms](https://img.shields.io/badge/Windows%20Forms-UI-lightgrey)](https://learn.microsoft.com/dotnet/desktop/winforms/) 
[![DevExpress](https://img.shields.io/badge/DevExpress-WinForms-orange)](https://www.devexpress.com/) 
[![SQLite](https://img.shields.io/badge/SQLite-Database-lightgrey?logo=sqlite)](https://www.sqlite.org/)

> **Logo J-Platform REST Service**, Logo J-Platform REST API ile tam entegre çalışan,  
> 📊 **muhasebe fişi (GL Slip)**, 📋 **Excel tabanlı toplu aktarım**, 🖼 **malzeme yönetimi**, 🤖 **AI ile görsel üretim** ve 🔑 **lisanslama** özelliklerini tek çatı altında sunan bir C#/.NET WinForms uygulamasıdır.  
> ✅ Token tabanlı güvenlik · ✅ Excel validasyon · ✅ AI entegrasyonu · ✅ Dinamik loglama · ✅ Çoklu veritabanı desteği  

---

## 🚀 Özellikler
- 🔐 **Oturum Yönetimi & Lisanslama** → Token tabanlı login/logout, otomatik yenileme, lisans doğrulama  
- 📊 **Genel Muhasebe (GL Slip)** → Fiş oluşturma, gün bazlı fiş (GLSlipDay), Excel import, grup / grup çöz  
- 🖼 **AI & Görsel Yönetimi** → Stability AI ile görsel üret, dosyadan toplu resim ekle, Gemini API ile açıklama çeviri  
- 🗂 **Malzeme Yönetimi** → Malzemeleri listele, toplu görsel güncelle, ERP/J-Platform’a tek tıkla aktar  
- ⚙️ **Sistem Ayarları** → REST bağlantı, SQL bağlantı, modül ayar ekranları  
- 📝 **Log & İzleme** → İşlem bazlı log ekranı, SQLite hata/günlük kaydı, kullanıcı hata kayıt raporu  
- 🎨 **Kullanıcı Deneyimi** → DevExpress grid, modern UI, tema desteği (dark/light), çoklu dil (TR/EN)  

---

## 📂 Proje Yapısı
```yaml
Logo_J-Platform_Rest_Service/
  ├── Forms/
  │   ├── GLAccountForm.cs            # Muhasebe hesap ekranı
  │   ├── GLSlipForm.cs               # Genel muhasebe fişi ekranı
  │   ├── DayGLSlipForm.cs            # Günlük fiş aktarım ekranı
  │   ├── ImageGenerateSettingForm.cs # AI görsel üretim ayarları
  │   ├── ItemsFileImageForm.cs       # Dosyadan toplu resim atama
  │   ├── LicenceInputForm.cs         # Lisans girişi
  │   ├── RestServiceSettingForm.cs   # REST servis ayarları
  │   ├── SQLSettingForm.cs           # SQL bağlantı ayarları
  │   └── LogsForm.cs                 # Log ekranı
  │
  ├── Helper/
  │   ├── JPlatformRest.cs            # REST API servisleri
  │   ├── JPlatformSession.cs         # Oturum yönetimi
  │   ├── SQLCrud.cs / SQLiteCrud.cs  # SQL işlemleri
  │   ├── GeminiTranslator.cs         # Google Gemini API
  │   ├── ImageCreateAI.cs            # Stability AI entegrasyonu
  │   ├── MachineIdHelper.cs          # Donanım ID hesaplama
  │   ├── EncryptionHelper.cs         # Güvenli şifreleme
  │   └── TextLog.cs                  # Log kaydı
  │
  ├── Bussines/
  │   ├── GLSlip/                     # Fiş yönetim class’ları
  │   ├── GLSlipDay/                  # Günlük fiş yönetimi
  │   ├── ExcelHeaderValidator.cs     # Excel format doğrulama
  │   ├── ImageSettingsValidator.cs   # Görsel ayar doğrulama
  │   └── SQLSettingsValidator.cs     # SQL ayar doğrulama
  │
  ├── Resources/                      # Excel şablonları, ikonlar
  ├── SQLite/                         # Ayar DB’si (RestSettings.db)
  └── Program.cs                      # Uygulama giriş noktası
```

---

## 🏃‍♂️ Kullanım Akışı
1️⃣ **Login / Lisans Girişi** → REST + SQL bağlantı ayarlarını yap, lisans anahtarını gir.  
2️⃣ **Genel Muhasebe** → Excel’den fiş yükle, kontrol et, Logo’ya aktar.  
3️⃣ **Malzeme Yönetimi** → Malzemeleri listele, dosya veya AI ile görselleri ekle.  
4️⃣ **AI İşlemleri** → Stability AI ile prompt gir, görsel üret; Gemini ile açıklamaları çevir.  
5️⃣ **Aktarım** → ERP/JPlatform’a tek tıkla gönder.  
6️⃣ **Loglama** → Başarılı/Uyarı/Hata kayıtlarını Log ekranından takip et.  

---

## 🎯 Durum Renkleri
- 🟢 **Başarılı** → Aktarım tamamlandı  
- 🟡 **Uyarı** → Veri eşleşti ama dönüştürülerek aktarıldı  
- 🔴 **Hata** → Aktarım/bağlantı hatası (detay log ekranında)  

---

## 🔧 Teknik Detaylar
- **Framework:** .NET Framework 4.8  
- **UI Library:** DevExpress WinForms 23.x  
- **Database:** SQLite (ayar + log + cache)  
- **Excel Processing:** EPPlus  
- **API:** Logo J-Platform REST API  
- **AI:** Stability AI (image), Google Gemini API (translate)  
- **Logging:** NLog + SQLite  

---

## 🐛 Bilinen Sorunlar
- Token süresi dolarsa → uygulama otomatik yeniler, devam etmezse tekrar login.  
- Excel yanlış format → Validator hata raporu verir.  
- Görsel eşleşmezse → AI fallback ile üretim yapılır.  

---

## 🏷️ Versiyon Geçmişi
### v3.0.0 (Güncel)
- ✅ Cari Kart modülü çıkarıldı  
- ✅ GLSlip + GLSlipDay entegrasyonu  
- ✅ Malzeme Yönetimi modülü eklendi  
- ✅ Stability AI + Gemini API entegrasyonu  
- ✅ Lisanslama ve sistem ayarları eklendi  
- ✅ Loglama & hata takip ekranları  

### v2.1.0
- Muhasebe fişi modülü, Excel aktarım, hata yönetimi  

### v2.0.0
- DevExpress UI entegrasyonu, token auth, SQLite  

### v1.0.0
- Temel cari kart işlemleri, REST API bağlantısı  

---

## 📜 Lisans
Bu proje **MIT License** ile lisanslanmıştır. [📄 Lisans Dosyasını Görüntüle](LICENSE)

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

## 📸 Ekran Görüntüleri

### Genel Muhasebe Aktarım
<img src="https://github.com/user-attachments/assets/7bb03be3-926f-46c7-9351-6ea878343204" width="800" />

### Malzeme Yönetimi
<img src="https://github.com/user-attachments/assets/555b42a3-81f1-41c6-9543-05c4b95a1002" width="800" />
