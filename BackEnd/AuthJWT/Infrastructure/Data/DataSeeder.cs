using AuthJWT.Domain.Entities.Common;
using AuthJWT.Domain.Entities.Security;
using AuthJWT.Domain.Enums;
using AuthJWT.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;

namespace AuthJWT.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task Seedata(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await SeedRolesData(scope);
                await SeedUserData(scope, context);
                // SeedRoomTypeData(context);
                // SeedLocationData(context);
                // SeedConvenienceData(context);

            }
        }
        public static async Task SeedRolesData(IServiceScope scope)
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new List<string> { "Admin", "User", "HotelOwner" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }


        private static async Task SeedUserData(IServiceScope scope, ApplicationDbContext context)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            for (int i = 37; i <= 39; i++)
            {
                // Create diverse locations using modulo to distribute across major cities
                string location = (i % 5) switch
                {
                    0 => "Hồ Chí Minh",
                    1 => "Hà Nội",
                    2 => "Đà Nẵng",
                    3 => "Nha Trang",
                    _ => "Phú Quốc"
                };

                // Create varied names
                // Create varied names
                string firstName = $"Owner{i}";
                string lastName = (i % 3) switch
                {
                    0 => "Nguyễn",
                    1 => "Trần",
                    _ => "Lê"
                };
                var hotelOwner = new ApplicationUser
                {
                    UserName = $"hotelowner{i}",
                    Email = $"hotelowner{i}@gmail.com",
                    DateOfBirth = DateTime.Now.AddYears(-30 - i % 30),
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = $"09{i.ToString().PadLeft(8, '0')}",
                    Gender = i % 2 == 0 ? "Male" : "Female",
                    Location = location,
                    IdentifyNumber = $"{100000000 + i}",
                    isActive = true,
                    isHotelOwner = true,
                    Avatar = "",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(hotelOwner, "Test123@");
                await userManager.AddToRoleAsync(hotelOwner, "HotelOwner");
            }
            // if (!context.ApplicationUsers.Any())
            // {
            //     // Create roles if they don't exist
            //     var adminRole = await roleManager.FindByNameAsync("Admin");
            //     var userRole = await roleManager.FindByNameAsync("User");
            //     var hotelOwnerRole = await roleManager.FindByNameAsync("HotelOwner");

            //     if (adminRole == null)
            //     {
            //         await roleManager.CreateAsync(new IdentityRole("Admin"));
            //     }
            //     if (userRole == null)
            //     {
            //         await roleManager.CreateAsync(new IdentityRole("User"));
            //     }
            //     if (hotelOwnerRole == null)
            //     {
            //         await roleManager.CreateAsync(new IdentityRole("HotelOwner"));
            //     }

            //     // Create Admin user
            //     var adminUser = new ApplicationUser
            //     {
            //         UserName = "admin",
            //         Email = "admin@gmail.com",
            //         DateOfBirth = DateTime.Now.AddYears(-30),
            //         FirstName = "Admin",
            //         LastName = "User",
            //         PhoneNumber = "0901234567",
            //         Gender = "Male",
            //         Location = "Ho Chi Minh City",
            //         IdentifyNumber = "123456789",
            //         isActive = true,
            //         Avatar = "",
            //         EmailConfirmed = true
            //     };
            //     await userManager.CreateAsync(adminUser, "Test123@");
            //     await userManager.AddToRoleAsync(adminUser, "Admin");

            //     // Create 5 regular users
            //     for (int i = 1; i <= 5; i++)
            //     {
            //         var user = new ApplicationUser
            //         {
            //             UserName = $"user{i}",
            //             Email = $"user{i}@gmail.com",
            //             DateOfBirth = DateTime.Now.AddYears(-25 - i),
            //             FirstName = $"User{i}",
            //             LastName = "Customer",
            //             PhoneNumber = $"090123456{i}",
            //             Gender = i % 2 == 0 ? "Female" : "Male",
            //             Location = "Hanoi",
            //             IdentifyNumber = $"12345678{i}",
            //             isActive = true,
            //             Avatar = "",
            //             EmailConfirmed = true
            //         };
            //         await userManager.CreateAsync(user, "Test123@");
            //         await userManager.AddToRoleAsync(user, "User");
            //     }

            //     // Create 5 hotel owner users

            // }
        }
        private static void SeedRoomTypeData(ApplicationDbContext context)
        {
            if (context.RoomTypes.Any()) return;

            var roomTypes = new List<RoomType>
            {
                new RoomType { RoomTypeName = "Phòng Tiêu Chuẩn", ShortDescription = "Phòng cơ bản với tiện nghi thiết yếu và nội thất thoải mái" },
                new RoomType { RoomTypeName = "Phòng Deluxe", ShortDescription = "Phòng rộng rãi với tiện nghi nâng cấp và tầm nhìn đẹp hơn" },
                new RoomType { RoomTypeName = "Phòng Superior", ShortDescription = "Phòng nâng cao với tính năng cao cấp và tiện nghi hiện đại" },
                new RoomType { RoomTypeName = "Suite Hành Chính", ShortDescription = "Suite sang trọng với khu vực sinh hoạt riêng và tiện nghi kinh doanh" },
                new RoomType { RoomTypeName = "Suite Tổng Thống", ShortDescription = "Chỗ ở hàng đầu với dịch vụ độc quyền và vị trí cao cấp" },
                new RoomType { RoomTypeName = "Phòng Gia Đình", ShortDescription = "Phòng lớn được thiết kế cho gia đình với nhiều giường và không gian rộng" },
                new RoomType { RoomTypeName = "Phòng Đôi", ShortDescription = "Phòng có hai giường đơn riêng biệt lý tưởng cho bạn bè hoặc đồng nghiệp" },
                new RoomType { RoomTypeName = "Phòng King", ShortDescription = "Phòng có giường cỡ king với trang trí thanh lịch và tiện nghi" },
                new RoomType { RoomTypeName = "Phòng View Biển", ShortDescription = "Phòng có tầm nhìn tuyệt đẹp ra biển và tiện nghi ven biển cao cấp" },
                new RoomType { RoomTypeName = "Suite Penthouse", ShortDescription = "Chỗ ở sang trọng tối đa với tầm nhìn toàn cảnh và quyền truy cập độc quyền" }
            };

            context.RoomTypes.AddRange(roomTypes);
            context.SaveChanges();
        }



        private static void SeedConvenienceData(ApplicationDbContext context)
        {
            if (context.Conveniences.Any()) return; // Đã có dữ liệu thì bỏ qua

            var conveniences = new List<Convenience>
            {
                // Room Conveniences
                new Convenience { Name = "Điều hòa không khí", Description = "Hệ thống kiểm soát khí hậu để duy trì nhiệt độ phòng thoải mái", Type = ConvenienceType.Room },
                new Convenience { Name = "WiFi miễn phí", Description = "Truy cập internet không dây miễn phí", Type = ConvenienceType.Room },
                new Convenience { Name = "TV màn hình phẳng", Description = "Tivi hiện đại với các kênh truyền hình cáp", Type = ConvenienceType.Room },
                new Convenience { Name = "Tủ lạnh mini", Description = "Tủ lạnh trong phòng với đồ uống và đồ ăn nhẹ", Type = ConvenienceType.Room },
                new Convenience { Name = "Phòng tắm riêng", Description = "Phòng tắm khép kín với vòi hoa sen và đồ vệ sinh", Type = ConvenienceType.Room },
                new Convenience { Name = "Két an toàn", Description = "Két sắt trong phòng để bảo quản đồ có giá trị", Type = ConvenienceType.Room },
                new Convenience { Name = "Ban công", Description = "Không gian ngoài trời riêng tư với khu vực ngồi", Type = ConvenienceType.Room },
                new Convenience { Name = "Dịch vụ phòng", Description = "Dịch vụ giao đồ ăn và thức uống 24 giờ", Type = ConvenienceType.Room },
                new Convenience { Name = "Bàn làm việc", Description = "Không gian làm việc chuyên dụng với ghế và đèn chiếu sáng", Type = ConvenienceType.Room },
                new Convenience { Name = "Đồ vệ sinh miễn phí", Description = "Tiện nghi phòng tắm và khăn tắm miễn phí", Type = ConvenienceType.Room },
                
                // Hotel Conveniences
                new Convenience { Name = "Hồ bơi", Description = "Hồ bơi ngoài trời hoặc trong nhà để khách thư giãn", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Phòng tập thể hình", Description = "Phòng gym được trang bị đầy đủ thiết bị tập luyện hiện đại", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Nhà hàng", Description = "Cơ sở ăn uống tại chỗ phục vụ ẩm thực địa phương và quốc tế", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Trung tâm Spa & Chăm sóc sức khỏe", Description = "Dịch vụ spa chuyên nghiệp và các liệu pháp chăm sóc sức khỏe", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Trung tâm kinh doanh", Description = "Không gian làm việc chuyên nghiệp với phòng họp và dịch vụ văn phòng", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Dịch vụ lễ tân", Description = "Hỗ trợ cá nhân cho việc đặt chỗ và gợi ý địa phương", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Bãi đỗ xe", Description = "Không gian đỗ xe an toàn cho phương tiện của khách", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Xe đưa đón sân bay", Description = "Dịch vụ vận chuyển miễn phí đến và từ sân bay", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Phòng hội nghị", Description = "Không gian họp và tổ chức sự kiện chuyên nghiệp", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Lễ tân 24 giờ", Description = "Dịch vụ lễ tân và chăm sóc khách hàng suốt ngày đêm", Type = ConvenienceType.Hotel }
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
