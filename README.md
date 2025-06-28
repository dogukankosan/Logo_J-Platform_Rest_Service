
# 🔗 Logo J-Platform REST Service

![ss](https://github.com/user-attachments/assets/3026dcc9-e4eb-4ad1-ad7e-a1573f412dbc)

![License](https://img.shields.io/github/license/dogukankosan/Logo_J-Platform_Rest_Service)
![Stars](https://img.shields.io/github/stars/dogukankosan/Logo_J-Platform_Rest_Service)
![Issues](https://img.shields.io/github/issues/dogukankosan/Logo_J-Platform_Rest_Service)
![Last Commit](https://img.shields.io/github/last-commit/dogukankosan/Logo_J-Platform_Rest_Service)

> **Logo_J-Platform_Rest_Service**, Logo J-Platform REST API ile entegre çalışan, token tabanlı oturum yönetimi, cari kart oluşturma/güncelleme ve durum değiştirme işlemlerini destekleyen bir masaüstü C#/.NET uygulamasıdır.

---

## 🚀 Özellikler

- 🔐 Token tabanlı oturum başlatma ve sonlandırma
- 🧾 Cari kart oluşturma (POST `/v2.0/arps`)
- 🔄 Aktif/Pasif durumu değiştirme (PUT `/v2.0/arps/status`)
- 📦 SQLite ile ayar veritabanı saklama
- 💬 DevExpress UI ile kullanıcı dostu arayüz
- 📡 JSON tabanlı dinamik veri gönderimi

---

## 🗂 Proje Yapısı

```
Logo_J-Platform_Rest_Service/
├── Forms/
│   ├── ArpCreateForm.cs         # Cari oluşturma ekranı
│   └── ArpListForm.cs           # Cari listeleme ekranı
├── Models/
│   ├── Arp.cs                   # Cari model sınıfı
│   └── SessionResult.cs         # Oturum sonuç modeli
├── Helpers/
│   ├── JPlatformSessionManager.cs  # Login/Logout yönetimi
│   └── ArpValidator.cs             # Alan doğrulama sınıfı
├── SQLite/
│   └── RestSettings.db           # Token ve bağlantı bilgileri
├── Program.cs                    # Giriş noktası
└── README.md                     # Bu döküman
```

---

## 🛠️ Kurulum & Çalıştırma

1. **Projeyi Klonla**
   ```bash
   git clone https://github.com/dogukankosan/Logo_J-Platform_Rest_Service.git
   cd Logo_J-Platform_Rest_Service
   ```

2. **Bağlantı Ayarlarını Yap**
   - İlk açılışta form üzerinden `RestSettings` bilgilerini gir (URL, kullanıcı adı, şifre).
   - Bu bilgiler SQLite veritabanına kaydedilir ve sonraki oturumlarda otomatik kullanılır.

3. **Projeyi Visual Studio ile aç ve F5 ile çalıştır**

---

## ⚙️ Kullanım Akışı

1. Uygulama açılırken `RestSettings` SQLite üzerinden okunur  
2. `StartSessionAsync()` çağrısı ile J-Platform’dan token alınır  
3. `POST /v2.0/arps` → JSON body ile cari kart eklenir  
4. Gerekirse `PUT /v2.0/arps/status?arpCode=XYZ` çağrısı ile pasif/aktif durumu değiştirilir  
5. Uygulama kapanırken `EndSessionAsync()` ile logout işlemi yapılır

---

## 📡 Desteklenen API Endpoint'leri

| Endpoint                     | Açıklama                           |
|-----------------------------|------------------------------------|
| `POST /auth/login`          | Oturum başlatma                    |
| `POST /v2.0/arps`           | Yeni cari kart oluşturma           |
| `PUT /v2.0/arps/status`     | Cariyi aktif/pasif yapma (arpCode) |
| `PUT /v2.0/arps/status/ref` | LogicalRef ile durum güncelleme    |
| `POST /auth/logout`         | Oturumu sonlandırma                |

---

## 📸 Ekran Görüntüleri

> Ekran görüntüleri eklenecektir.

---

## 🤝 Katkı

Katkı yapmak istersen projeyi forklayıp geliştirme yapabilir ve pull request gönderebilirsin.  
Soruların ya da hata bildirimlerin için [Issues sekmesi](https://github.com/dogukankosan/Logo_J-Platform_Rest_Service/issues) her zaman açık.

---

## 📄 Lisans

MIT License © [@dogukankosan](https://github.com/dogukankosan)

---

## 📬 İletişim

- 👨‍💻 Geliştirici: [@dogukankosan](https://github.com/dogukankosan)

---

<p align="center">
  <img src="https://img.shields.io/badge/.NET-Framework-blue?logo=dotnet" alt=".NET Framework" />
  <img src="https://img.shields.io/badge/SQLite-Database-lightgrey" alt="SQLite" />
  <img src="https://img.shields.io/badge/DevExpress-UI-orange" alt="DevExpress" />
</p>
