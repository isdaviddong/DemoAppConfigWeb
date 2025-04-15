# Azure App Config WebApp(3.1) Demo for AZ-400 AZ-204
### 使用app configuration做微服務應用程式的環境變數管理
上課時學員問到，以前早期都是把應用程式的環境變數存放在 appconfig.json 或 web.config中，為何現在要用 Azure App Configuration Services 來統籌管理? 

原因很簡單，因為『微服務』的出現。

過去，應用程式可能是以一個巨大的單體進行佈署的(例如ERP、CRM)，而如今，應用程式開始被切分為小小塊的個別微服務，每個服務又有各自的環境變數設定。但如果要集合這些服務來完成一個完整的工作，配置環境變數的這個任務可能就得需要一起同時進行。

此外，還有一個重要的原因就是，如今的應用程式封裝與佈署，很可能透過容器化的方式來進行。那應用程式已經被壓成一個 image了，事後要去修改和調整環境變數就變得不方便。這時候，透過另一個統一的 "環境變數管理服務" 來配置環境變數，就顯得順理成章也理所當然了...

底下這個影片，花五分鐘的時間，來借數如何使用 Azure 上的Configuration Services 搭配 .net core 的開發技術做分散式應用程式的環境變數使用與管理...

https://youtu.be/X1BmflVR7Fc


---

## 程式功能說明

### 1. Azure App Configuration
Azure App Configuration 是一個專門用來集中管理應用程式設定的服務。它的主要功能包括：
- **集中化管理**：將所有應用程式的設定集中存放，避免分散在多個檔案中。
- **動態更新**：支援即時更新設定，無需重新部署應用程式。
- **版本控制**：可以追蹤設定的歷史版本，方便回滾。
- **安全性**：透過 Azure 的存取控制機制，確保設定的安全性。

在本專案中，我們使用 Azure App Configuration 來管理微服務的環境變數，例如：
- 資料庫連線字串
- API 金鑰
- 第三方服務的 URL

程式會透過 `.NET Core` 的 `Microsoft.Extensions.Configuration.AzureAppConfiguration` 套件來讀取這些設定，並將其注入到應用程式中。

### 2. Feature Toggle
Feature Toggle（功能開關）是一種軟體設計模式，用來控制應用程式中某些功能是否啟用。這在以下情境中特別有用：
- **A/B 測試**：針對不同的使用者群體啟用不同的功能。
- **漸進式部署**：逐步釋出新功能，降低風險。
- **快速回滾**：在功能出現問題時，快速關閉功能。

在本專案中，我們使用 Azure App Configuration 的 **Feature Management** 功能來實現 Feature Toggle。以下是實作的步驟：
1. 在 Azure App Configuration 中定義功能標誌（Feature Flags）。
2. 使用 `.NET Core` 的 `Microsoft.FeatureManagement` 套件來讀取這些功能標誌。
3. 在程式碼中透過條件判斷來啟用或停用特定功能。

範例程式碼：
```csharp
// filepath: d:\test\DemoAppConfigWeb\Program.cs
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// 註冊 Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect("Your_Connection_String")
           .UseFeatureFlags();
});

// 註冊 Feature Management
builder.Services.AddFeatureManagement();

var app = builder.Build();

// 使用 Feature Toggle
app.MapGet("/", async (IFeatureManager featureManager) =>
{
    if (await featureManager.IsEnabledAsync("NewFeature"))
    {
        return Results.Ok("新功能已啟用!");
    }
    else
    {
        return Results.Ok("新功能尚未啟用!");
    }
});

app.Run();
```