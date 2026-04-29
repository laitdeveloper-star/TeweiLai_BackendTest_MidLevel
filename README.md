# Backend Test - Mid Level

本專案為後端工程師技術測試實作，內容包含：

- `.NET 8 Web API`
- `MyOffice_ACPD` CRUD API
- SQL Server 建表與 Stored Procedure 腳本

## 專案結構

- `BackendExamHub.Api/`
  Web API 專案
- `database/00_setup.sql`
  建立資料表與 Stored Procedure
- `TSQLScript/`
  題目原始提供檔案

## 執行環境

- .NET 8 SDK
- SQL Server 2019 以上
- SSMS 22

## 資料庫建立步驟

1. 在 SQL Server 建立資料庫 `BackendExamHub`
2. 執行 `database/00_setup.sql`
3. 視需要新增測試資料

## API 啟動方式

1. 修改 `BackendExamHub.Api/appsettings.json` 內的連線字串
2. 在專案根目錄執行：

```powershell
dotnet restore
dotnet run --project .\BackendExamHub.Api
```

啟動後可於 Swagger 測試：

```text
https://localhost:7215/swagger
http://localhost:5215/swagger
```

## API 路由

```http
GET    /api/myofficeacpd
GET    /api/myofficeacpd/{sid}
POST   /api/myofficeacpd
PUT    /api/myofficeacpd/{sid}
DELETE /api/myofficeacpd/{sid}
```

## 補充說明

- `ACPD_SID` 由 `dbo.NEWSID` 產生
- 新增、修改、刪除會呼叫 `dbo.usp_AddLog` 記錄操作
- 資料表欄位以題目提供的 `.sql` 腳本為準
