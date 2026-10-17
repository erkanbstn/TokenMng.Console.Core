# Token Management

Omerd iş uygulamaları için OAuth2 erişim token'ı alan ve yenileyen konsol programı.

## Ne var

HTTP isteği atar, gelen JSON içinden token bilgisini okur ve süresi dolunca yeniler.

## Çalıştırma

```powershell
dotnet run --project TokenManagement
```

## Yığın

.NET 9 ve Newtonsoft.Json.