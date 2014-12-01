namespace ShareLink.Models

type ShareData = { Title: string; Uri: System.Uri; ApplicationName: string }  with
    member this.WithTitle title = { this with Title = title }
    member this.WithApplicationName applicationName = { this with ApplicationName = applicationName }

