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
                SeedRoomTypeData(context);
                SeedLocationData(context);
                SeedConvenienceData(context);

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
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!context.ApplicationUsers.Any())
            {
                // Create roles if they don't exist
                var adminRole = await roleManager.FindByNameAsync("Admin");
                var userRole = await roleManager.FindByNameAsync("User");
                var hotelOwnerRole = await roleManager.FindByNameAsync("HotelOwner");

                if (adminRole == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (userRole == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }
                if (hotelOwnerRole == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("HotelOwner"));
                }

                // Create Admin user
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "0901234567",
                    Gender = "Male",
                    Location = "Ho Chi Minh City",
                    IdentifyNumber = "123456789",
                    isActive = true,
                    Avatar = "",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Test123@");
                await userManager.AddToRoleAsync(adminUser, "Admin");

                // Create 5 regular users
                for (int i = 1; i <= 5; i++)
                {
                    var user = new ApplicationUser
                    {
                        UserName = $"user{i}",
                        Email = $"user{i}@gmail.com",
                        DateOfBirth = DateTime.Now.AddYears(-25 - i),
                        FirstName = $"User{i}",
                        LastName = "Customer",
                        PhoneNumber = $"090123456{i}",
                        Gender = i % 2 == 0 ? "Female" : "Male",
                        Location = "Hanoi",
                        IdentifyNumber = $"12345678{i}",
                        isActive = true,
                        Avatar = "",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, "Test123@");
                    await userManager.AddToRoleAsync(user, "User");
                }

                // Create 5 hotel owner users
                for (int i = 1; i <= 5; i++)
                {
                    var hotelOwner = new ApplicationUser
                    {
                        UserName = $"hotelowner{i}",
                        Email = $"hotelowner{i}@gmail.com",
                        DateOfBirth = DateTime.Now.AddYears(-35 - i),
                        FirstName = $"Owner{i}",
                        LastName = "Hotel",
                        PhoneNumber = $"090876543{i}",
                        Gender = i % 2 == 0 ? "Male" : "Female",
                        Location = "Da Nang",
                        IdentifyNumber = $"98765432{i}",
                        isActive = true,
                        isHotelOwner = true,
                        Avatar = "",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(hotelOwner, "Test123@");
                    await userManager.AddToRoleAsync(hotelOwner, "HotelOwner");
                }
            }
        }
        private static void SeedRoomTypeData(ApplicationDbContext context)
        {
            if (context.RoomTypes.Any()) return;

            var roomTypes = new List<RoomType>
            {
                new RoomType { RoomTypeName = "Standard Room", ShortDescription = "Basic room with essential amenities and comfortable furnishing" },
                new RoomType { RoomTypeName = "Deluxe Room", ShortDescription = "Spacious room with upgraded amenities and better view" },
                new RoomType { RoomTypeName = "Superior Room", ShortDescription = "Enhanced room with premium features and modern facilities" },
                new RoomType { RoomTypeName = "Executive Suite", ShortDescription = "Luxurious suite with separate living area and business amenities" },
                new RoomType { RoomTypeName = "Presidential Suite", ShortDescription = "Top-tier accommodation with exclusive services and premium location" },
                new RoomType { RoomTypeName = "Family Room", ShortDescription = "Large room designed for families with multiple beds and extra space" },
                new RoomType { RoomTypeName = "Twin Room", ShortDescription = "Room with two separate single beds ideal for friends or colleagues" },
                new RoomType { RoomTypeName = "King Room", ShortDescription = "Room featuring a king-size bed with elegant decor and amenities" },
                new RoomType { RoomTypeName = "Ocean View Room", ShortDescription = "Room with stunning ocean views and premium coastal amenities" },
                new RoomType { RoomTypeName = "Penthouse Suite", ShortDescription = "Ultimate luxury accommodation with panoramic views and exclusive access" }
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
                new Convenience { Name = "Air Conditioning", Description = "Climate control system for comfortable room temperature", Type = ConvenienceType.Room },
                new Convenience { Name = "Free WiFi", Description = "Complimentary wireless internet access", Type = ConvenienceType.Room },
                new Convenience { Name = "Flat Screen TV", Description = "Modern television with cable channels", Type = ConvenienceType.Room },
                new Convenience { Name = "Mini Bar", Description = "In-room refrigerator with beverages and snacks", Type = ConvenienceType.Room },
                new Convenience { Name = "Private Bathroom", Description = "En-suite bathroom with shower and toiletries", Type = ConvenienceType.Room },
                new Convenience { Name = "Safe Box", Description = "In-room security safe for valuables", Type = ConvenienceType.Room },
                new Convenience { Name = "Balcony", Description = "Private outdoor space with seating area", Type = ConvenienceType.Room },
                new Convenience { Name = "Room Service", Description = "24-hour food and beverage delivery service", Type = ConvenienceType.Room },
                new Convenience { Name = "Work Desk", Description = "Dedicated workspace with chair and lighting", Type = ConvenienceType.Room },
                new Convenience { Name = "Complimentary Toiletries", Description = "Free bathroom amenities and towels", Type = ConvenienceType.Room },
                
                // Hotel Conveniences
                new Convenience { Name = "Swimming Pool", Description = "Outdoor or indoor pool for guests relaxation", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Fitness Center", Description = "Fully equipped gym with modern exercise equipment", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Restaurant", Description = "On-site dining facility serving local and international cuisine", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Spa & Wellness Center", Description = "Professional spa services and wellness treatments", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Business Center", Description = "Professional workspace with meeting rooms and office services", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Concierge Service", Description = "Personal assistance for reservations and local recommendations", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Parking Facility", Description = "Secure parking space for guests vehicles", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Airport Shuttle", Description = "Complimentary transportation to and from airport", Type = ConvenienceType.Hotel },
                new Convenience { Name = "Conference Rooms", Description = "Professional meeting and event spaces", Type = ConvenienceType.Hotel },
                new Convenience { Name = "24-Hour Front Desk", Description = "Round-the-clock reception and guest services", Type = ConvenienceType.Hotel }
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
