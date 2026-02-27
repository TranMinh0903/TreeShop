# Sử dụng SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# 1. Copy các file .csproj vào đúng thư mục tương ứng trong Docker để Restore
# Đã cập nhật từ LaptopShop -> TreeShop
COPY ["PRN232.TreeShop.API/PRN232.TreeShop.API.csproj", "PRN232.TreeShop.API/"]
COPY ["PRN232.TreeShop.Repo/PRN232.TreeShop.Repo.csproj", "PRN232.TreeShop.Repo/"]
COPY ["PRN232.TreeShop.Services/PRN232.TreeShop.Services.csproj", "PRN232.TreeShop.Services/"]

# 2. Chạy lệnh restore cho project chính
RUN dotnet restore "PRN232.TreeShop.API/PRN232.TreeShop.API.csproj"

# 3. Copy toàn bộ code vào Docker
COPY . .

# 4. Build và Publish project chính
WORKDIR "/app/PRN232.TreeShop.API"
RUN dotnet publish -c Release -o /app/out

# Giai đoạn chạy (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Chạy file DLL của project chính (Lưu ý: Tên file DLL phải khớp với tên Project)
ENTRYPOINT ["dotnet", "PRN232.TreeShop.API.dll"]