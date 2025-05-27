using AuthJWT.Domain.Entities.Common;
using AuthJWT.Infrastructure.Context;

namespace AuthJWT.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task Seedata(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                // context.Database.Migrate();
                // if (!context.ApplicationUsers.Any())
                // {
                //     var user = new ApplicationUser
                //     {
                //         UserName = "admin",
                //         Email = "admin@gmail.com",
                //         DateOfBirth = DateTime.Now.AddYears(-20),
                //         FirstName = "Admin",
                //         LastName = "User",
                //         PhoneNumber = "1234567890",
                //         Gender = "Male",
                //         Location = "test",
                //         IdentifyNumber = "123456789",
                //         isActive = true,
                //         Avatar = ""
                //     };
                // var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                // var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                //     if (!await roleManager.RoleExistsAsync("Admin"))
                //     {
                //         var role = new IdentityRole
                //         {
                //             Name = "Admin",
                //             NormalizedName = "ADMIN"
                //         };
                //         var role2 = new IdentityRole
                //         {
                //             Name = "User",
                //             NormalizedName = "USER"
                //         };
                //         await roleManager.CreateAsync(role);
                //         await roleManager.CreateAsync(role2);
                //     }

                // await roleManager.CreateAsync(new IdentityRole("HotelOwner"));
                //     await userManager.CreateAsync(user, "Admin@123");
                //     await userManager.AddToRoleAsync(user, "Admin");
                // }
                // SeedLocationData(context);
                // SeedHotelData(context);
                SeedConvenienceData(context);

            }
        }
        private static void SeedConvenienceData(ApplicationDbContext context)
        {
            if (context.Conveniences.Any()) return; // Đã có dữ liệu thì bỏ qua

            var conveniences = new List<Convenience>
            {
                new Convenience { Name = "Wifi", Description = "Wifi miễn phí" },
                new Convenience { Name = "Bể bơi", Description = "Bể bơi trong nhà" },
                new Convenience { Name = "Gym", Description = "Phòng tập gym" },
                new Convenience { Name = "Nhà hàng", Description = "Nhà hàng phục vụ ăn uống" },
                new Convenience { Name = "Spa", Description = "Dịch vụ spa và massage" },
                new Convenience { Name = "Đỗ xe miễn phí", Description = "Chỗ đỗ xe miễn phí" },
                new Convenience { Name = "Thang máy", Description = "Thang máy trong tòa nhà" },
                new Convenience { Name = "Dịch vụ phòng", Description = "Dịch vụ phục vụ tại phòng" },
                new Convenience { Name = "Tiệc nướng BBQ", Description = "Tiệc nướng BBQ ngoài trời" },
                new Convenience { Name = "Karaoke", Description = "Phòng karaoke" },
                new Convenience { Name = "Trung tâm hội nghị", Description = "Phòng họp và hội nghị" },
                new Convenience { Name = "Dịch vụ đưa đón sân bay", Description = "Dịch vụ đưa đón tại sân bay" },
                new Convenience { Name = "Giặt là", Description = "Dịch vụ giặt là" },
                new Convenience { Name = "Bãi biển riêng", Description = "Bãi biển riêng" },
                new Convenience { Name = "Khu vui chơi trẻ em", Description = "Khu vui chơi cho trẻ em" },
            };

            context.Conveniences.AddRange(conveniences);
            context.SaveChanges();
        }
        private static void SeedLocationData(ApplicationDbContext context)
        {
            if (context.Locations.Any()) return; // Đã có dữ liệu thì bỏ qua

            var vietnamCities = new List<Location>
        {
            new Location { City = "Hà Nội", Country = "Việt Nam", Latitude = 21.027763m, Longitude = 105.834160m },
            new Location { City = "Hồ Chí Minh", Country = "Việt Nam", Latitude = 10.823099m, Longitude = 106.629664m },
            new Location { City = "Đà Nẵng", Country = "Việt Nam", Latitude = 16.054407m, Longitude = 108.202167m },
            new Location { City = "Hải Phòng", Country = "Việt Nam", Latitude = 20.844912m, Longitude = 106.688084m },
            new Location { City = "Cần Thơ", Country = "Việt Nam", Latitude = 10.045162m, Longitude = 105.746857m },
            new Location { City = "An Giang", Country = "Việt Nam", Latitude = 10.521584m, Longitude = 105.125896m },
            new Location { City = "Bà Rịa - Vũng Tàu", Country = "Việt Nam", Latitude = 10.541670m, Longitude = 107.242997m },
            new Location { City = "Bắc Giang", Country = "Việt Nam", Latitude = 21.281982m, Longitude = 106.197477m },
            new Location { City = "Bắc Kạn", Country = "Việt Nam", Latitude = 22.141800m, Longitude = 105.829949m },
            new Location { City = "Bạc Liêu", Country = "Việt Nam", Latitude = 9.294003m, Longitude = 105.721566m },
            new Location { City = "Bắc Ninh", Country = "Việt Nam", Latitude = 21.183333m, Longitude = 106.050000m },
            new Location { City = "Bến Tre", Country = "Việt Nam", Latitude = 10.233333m, Longitude = 106.383333m },
            new Location { City = "Bình Định", Country = "Việt Nam", Latitude = 13.766666m, Longitude = 109.233333m },
            new Location { City = "Bình Dương", Country = "Việt Nam", Latitude = 11.325402m, Longitude = 106.477017m },
            new Location { City = "Bình Phước", Country = "Việt Nam", Latitude = 11.756041m, Longitude = 106.723463m },
            new Location { City = "Bình Thuận", Country = "Việt Nam", Latitude = 10.928888m, Longitude = 108.100000m },
            new Location { City = "Cà Mau", Country = "Việt Nam", Latitude = 9.176944m, Longitude = 105.150000m },
            new Location { City = "Cao Bằng", Country = "Việt Nam", Latitude = 22.666667m, Longitude = 106.250000m },
            new Location { City = "Đắk Lắk", Country = "Việt Nam", Latitude = 12.666667m, Longitude = 108.050000m },
            new Location { City = "Đắk Nông", Country = "Việt Nam", Latitude = 12.004230m, Longitude = 107.690739m },
            new Location { City = "Điện Biên", Country = "Việt Nam", Latitude = 21.383333m, Longitude = 103.016667m },
            new Location { City = "Đồng Nai", Country = "Việt Nam", Latitude = 10.950000m, Longitude = 106.816667m },
            new Location { City = "Đồng Tháp", Country = "Việt Nam", Latitude = 10.533333m, Longitude = 105.633333m },
            new Location { City = "Gia Lai", Country = "Việt Nam", Latitude = 13.983333m, Longitude = 108.000000m },
            new Location { City = "Hà Giang", Country = "Việt Nam", Latitude = 22.833333m, Longitude = 104.983333m },
            new Location { City = "Hà Nam", Country = "Việt Nam", Latitude = 20.541111m, Longitude = 105.913889m },
            new Location { City = "Hà Tĩnh", Country = "Việt Nam", Latitude = 18.333333m, Longitude = 105.900000m },
            new Location { City = "Hải Dương", Country = "Việt Nam", Latitude = 20.933333m, Longitude = 106.316667m },
            new Location { City = "Hậu Giang", Country = "Việt Nam", Latitude = 9.784444m, Longitude = 105.470000m },
            new Location { City = "Hòa Bình", Country = "Việt Nam", Latitude = 20.813333m, Longitude = 105.338333m },
            new Location { City = "Hưng Yên", Country = "Việt Nam", Latitude = 20.646389m, Longitude = 106.051944m },
            new Location { City = "Khánh Hòa", Country = "Việt Nam", Latitude = 12.250000m, Longitude = 109.183333m },
            new Location { City = "Kiên Giang", Country = "Việt Nam", Latitude = 10.000000m, Longitude = 105.083333m },
            new Location { City = "Kon Tum", Country = "Việt Nam", Latitude = 14.350000m, Longitude = 108.000000m },
            new Location { City = "Lai Châu", Country = "Việt Nam", Latitude = 22.400000m, Longitude = 103.450000m },
            new Location { City = "Lâm Đồng", Country = "Việt Nam", Latitude = 11.933333m, Longitude = 108.416667m },
            new Location { City = "Lạng Sơn", Country = "Việt Nam", Latitude = 21.833333m, Longitude = 106.733333m },
            new Location { City = "Lào Cai", Country = "Việt Nam", Latitude = 22.483333m, Longitude = 103.950000m },
            new Location { City = "Long An", Country = "Việt Nam", Latitude = 10.666667m, Longitude = 106.166667m },
            new Location { City = "Nam Định", Country = "Việt Nam", Latitude = 20.420000m, Longitude = 106.168333m },
            new Location { City = "Nghệ An", Country = "Việt Nam", Latitude = 19.333333m, Longitude = 104.833333m },
            new Location { City = "Ninh Bình", Country = "Việt Nam", Latitude = 20.250000m, Longitude = 105.966667m },
            new Location { City = "Ninh Thuận", Country = "Việt Nam", Latitude = 11.563837m, Longitude = 108.995697m },
            new Location { City = "Phú Thọ", Country = "Việt Nam", Latitude = 21.300000m, Longitude = 105.383333m },
            new Location { City = "Phú Yên", Country = "Việt Nam", Latitude = 13.083333m, Longitude = 109.300000m },
            new Location { City = "Quảng Bình", Country = "Việt Nam", Latitude = 17.483333m, Longitude = 106.600000m },
            new Location { City = "Quảng Nam", Country = "Việt Nam", Latitude = 15.883333m, Longitude = 108.333333m },
            new Location { City = "Quảng Ngãi", Country = "Việt Nam", Latitude = 15.116667m, Longitude = 108.800000m },
            new Location { City = "Quảng Ninh", Country = "Việt Nam", Latitude = 21.016667m, Longitude = 107.300000m },
            new Location { City = "Quảng Trị", Country = "Việt Nam", Latitude = 16.750000m, Longitude = 107.200000m },
            new Location { City = "Sóc Trăng", Country = "Việt Nam", Latitude = 9.602500m, Longitude = 105.973889m },
            new Location { City = "Sơn La", Country = "Việt Nam", Latitude = 21.316667m, Longitude = 103.900000m },
            new Location { City = "Tây Ninh", Country = "Việt Nam", Latitude = 11.300000m, Longitude = 106.100000m },
            new Location { City = "Thái Bình", Country = "Việt Nam", Latitude = 20.446111m, Longitude = 106.342222m },
            new Location { City = "Thái Nguyên", Country = "Việt Nam", Latitude = 21.592778m, Longitude = 105.844167m },
            new Location { City = "Thanh Hóa", Country = "Việt Nam", Latitude = 19.800000m, Longitude = 105.766667m },
            new Location { City = "Thừa Thiên Huế", Country = "Việt Nam", Latitude = 16.466667m, Longitude = 107.583333m },
            new Location { City = "Tiền Giang", Country = "Việt Nam", Latitude = 10.350000m, Longitude = 106.366667m },
            new Location { City = "Trà Vinh", Country = "Việt Nam", Latitude = 9.934722m, Longitude = 106.345278m },
            new Location { City = "Tuyên Quang", Country = "Việt Nam", Latitude = 21.816667m, Longitude = 105.216667m },
            new Location { City = "Vĩnh Long", Country = "Việt Nam", Latitude = 10.250000m, Longitude = 105.966667m },
            new Location { City = "Vĩnh Phúc", Country = "Việt Nam", Latitude = 21.308889m, Longitude = 105.604167m },
            new Location { City = "Yên Bái", Country = "Việt Nam", Latitude = 21.700000m, Longitude = 104.866667m }
        };

            context.Locations.AddRange(vietnamCities);
            context.SaveChanges();
        }
    }
}
