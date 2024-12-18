export PATH="$PATH:$HOME/.dotnet/tools/"export PATH="$PATH:$HOME/.dotnet/tools/"
dotnet ef database update 0
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update