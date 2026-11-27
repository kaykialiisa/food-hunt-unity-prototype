## Git iş akışı

- Öneriden önce mevcut dalı, commit edilmemiş değişiklikleri ve uzak dala göre gönderilmeyi bekleyen commit’leri kontrol et. Bir öneri gerekiyorsa işlemi ve nedenini tek kısa mesajda belirt.
- İşe başlamadan önce, çalışan bir mekaniği değiştirmek veya vazgeçilebilecek bir denemeyi ayırmak için anlamlı bir neden varsa branch öner. Küçük düzeltmeler ve mevcut yapıyı kullanan yeni level’lar aynı dalda ilerleyebilir.
- Kendi başına açıklanabilen bir değişiklik tamamlandığında commit öner. Neyin tamamlandığını, nasıl doğrulandığını ve kısa bir commit mesajını belirt.
- Çalışan bir kilometre taşında veya oturumu kapatacağımı söylediğimde, gönderilmemiş commit varsa push öner. Commit edilmemiş değişiklikleri ayrıca belirt.
- Ayrı branch’teki iş tamamlanıp doğrulandığında `main` dalına merge öner. Eksik test veya Unity oynanış kontrolü varsa hangisinin beklediğini söyle. PR ve `develop` dalı isteğe bağlıdır.
- Branch açma, commit, push ve merge için ilgili işleme açık onayımı bekle. Bir işlem için verdiğim onay yalnızca o işlemi kapsar.
