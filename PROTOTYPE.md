## Ana Mekanik

Oyun doğrudan 3D `Game` sahnesinde açılır. Ekranın üstünde renkli piksellerden oluşan bir resim, altında sınırlı sayıda çalışma slotu ve en altta sıralar hâlinde bekleyen karınca kutuları bulunur. Her kutunun rengi ve toplaması gereken kalan piksel sayısını gösteren bir sayacı vardır.

1. Oyuncu ön sıradaki seçilebilir bir kutuya dokunur. Boş slot varsa kutu sıradan ayrılıp slota yerleşir. Bu hareket, arkasındaki kutuları seçilebilir hâle getirebilir. Boş slot yoksa kutu sırada kalır.
2. Slota yerleşen kutudan çıkan karıncalar, resmin dışarıdan erişilebilen kenarındaki kendi renkleriyle eşleşen pikselleri otomatik toplar. Oyuncu yalnızca kutuyu seçer.
3. Kutuya teslim edilen her piksel sayacı bir azaltır. Sayaç sıfıra ulaştığında kutu alandan ayrılır ve slot boşalır.
4. Sayaç sıfıra ulaşmadan erişilebilir eşleşme kalmazsa kutu slotta bekler. Başka bir kutu uygun renkte piksellerin önünü açtığında bekleyen kutu yeni bir dokunuş gerektirmeden toplamaya devam eder.

Oyuncu kutu seçerken hem resmin açık kenarlarını hem de sırada bekleyen kutuları değerlendirir. Örneğin, mavi bir merkez kırmızı bir halkayla çevriliyse mavi kutuyu önce göndermek bir slotu beklemeye bağlar. Kırmızı kutu halkada bir açıklık oluşturduğunda mavi kutu da toplamaya başlayabilir. Bir hamlenin değeri hem topladığı piksellere hem de erişime açtığı renklere bağlıdır.

- **Kazanma.** Resimdeki son piksel temizlendiğinde oyuncu kazanır.
- **Kaybetme.** Resimde piksel kalmışsa, kullanılabilir slotların tamamı doluysa, hiçbir aktif kutu ilerleyemiyorsa ve sonucu değiştirebilecek bir toplama veya teslimat sürmüyorsa oyuncu kaybeder. Son boş slota yerleşen kutu ilerleyebiliyorsa oyun devam eder.
