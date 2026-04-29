# Backend Interview Mid

這份 repository 已整理成一個可交付的後端面試起手式，內容包含：

- `TSQLScript/`：題目提供的 SQL Server 資料表、SID 產生程序、Log 預存程序與 PDF 規格
- `BackendExamHub.Api/`：`.NET 8 Web API` 專案骨架，已實作 `MyOffice_ACPD` 的 CRUD 路由
- `database/00_setup.sql`：用來建立題目資料表與預存程序的初始化腳本

## 題目重點

根據 PDF 與原始 README，這份題目主要要求：

- 使用 `.NET 8 Web API`
- 使用 `SQL Server 2019+`
- 對 `dbo.MyOffice_ACPD` 實作 CRUD
- API 路由需符合 RESTful 設計
- 可透過 Swagger 測試 API
- 保留題目提供的 `NEWSID` 與 `usp_AddLog`

目前 API 路由如下：

```http
GET    /api/myofficeacpd
GET    /api/myofficeacpd/{sid}
POST   /api/myofficeacpd
PUT    /api/myofficeacpd/{sid}
DELETE /api/myofficeacpd/{sid}
```

## 專案結構

```text
BackendExamHub.Api/
database/
TSQLScript/
README.md
```

## 建立資料庫

1. 先建立資料庫 `BackendExamHub`
2. 使用 `sqlcmd` 或 SSMS 執行 [database/00_setup.sql](C:/Users/USER/Documents/Codex/2026-04-29-https-github-com-mercuryfire-backend-interview/database/00_setup.sql)
3. 確認以下物件已建立：
   - `dbo.MyOffice_ACPD`
   - `dbo.MyOffice_ExcuteionLog`
   - `dbo.NEWSID`
   - `dbo.usp_AddLog`

## 產生 `.bak`

`.bak` 必須從實際的 SQL Server 資料庫備份產生，不能只靠目前 repo 直接組出來。

1. 先在 SQL Server 建立並初始化 `BackendExamHub`
2. 視需要調整 [database/01_backup.sql](C:/Users/USER/Documents/Codex/2026-04-29-https-github-com-mercuryfire-backend-interview/database/01_backup.sql) 裡的備份路徑
3. 在 SSMS 或 `sqlcmd` 執行該腳本
4. 執行完成後會得到 `BackendExamHub.bak`

## 啟動 API

先調整 [BackendExamHub.Api/appsettings.json](C:/Users/USER/Documents/Codex/2026-04-29-https-github-com-mercuryfire-backend-interview/BackendExamHub.Api/appsettings.json) 內的 SQL Server 連線字串，再執行：

```powershell
dotnet restore
dotnet run --project .\BackendExamHub.Api
```

啟動後可在 Swagger 測試：

```text
https://localhost:xxxx/swagger
```

## 已完成內容

- 建立 `.NET 8 Web API` 專案骨架
- 加入 Swagger
- 建立 `MyOffice_ACPD` 的 Model / DTO / Repository / Controller
- 新增使用 `dbo.NEWSID` 產生 `ACPD_SID`
- 在新增、修改、刪除時呼叫 `dbo.usp_AddLog`

## 注意事項

題目提供的 PDF、README 與 SQL Script 之間有少量欄位規格不一致，例如：

- `ACPD_StopMemo` 長度在 PDF 與 SQL Script 不同
- `ACPD_Memo` 長度在 PDF 與 SQL Script 不同
- `MyOffice_ExcuteionLog` 主鍵欄位名稱在 PDF 與 SQL Script 不同

目前 API 以題目附的 `.sql` 檔案為主要實作依據，這樣比較接近實際建表結果。
