using Application.Interfaces;
using Domain;
using Domain.Entities;
using Domain.Entities.TestHub;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class SeederService(AppDbContext db)
    : ISeederService
{
    private const string CSharpTest = "Основи C#";
    private const string EfTest = "Entity Framework Core";

    private const string QIntType = "Який тип використовується для зберігання цілих чисел?";
    private const string QAsync = "Яке ключове слово використовується для асинхронних методів?";
    private const string QMigration = "Яка команда створює нову міграцію?";
    private const string QRelations = "Які атрибути Data Annotations використовуються для зв'язків? (оберіть усі правильні)";

    public async Task SeedVehicleStatusesAsync()
    {
        if (await db.VehicleStatuses.AnyAsync())
            return;

        var vStatuses = new List<VehicleStatusEntity>
        {
            new ()  { Name = "В наявності" },
            new ()  { Name = "Заброньовано" },
            new ()  { Name = "Продано" }
        };
        await db.VehicleStatuses.AddRangeAsync(vStatuses);
        await db.SaveChangesAsync();
    }

    public async Task SeedLeadStatusesAsync()
    {
        if (await db.LeadStatuses.AnyAsync())
            return;

        var lStatuses = new List<LeadStatusEntity>
        {
             new () { Name = "Нова" },
             new () { Name = "В роботі" },
             new () { Name = "Виконана" },
             new () { Name = "Відхилена" }
        };
        await db.LeadStatuses.AddRangeAsync(lStatuses);
        await db.SaveChangesAsync();
    }

    public async Task UpdateDatabase()
    {
        await db.Database.MigrateAsync();
    }

    public async Task SeedFuelTypesAsync()
    {
        if (await db.FuelTypes.AnyAsync())
            return;

        var entities = new List<FuelTypeEntity>
        {
             new () { Name = "Дизель" },
             new () { Name = "Бензин" },
             new () { Name = "Газ" },
             new () { Name = "Газ/бензин" },
             new () { Name = "Гібрид" },
             new () { Name = "Електро" }
        };
        await db.FuelTypes.AddRangeAsync(entities);
        await db.SaveChangesAsync();
    }

    public async Task SeedBodyTypesAsync()
    {
        if (await db.BodyTypes.AnyAsync())
            return;

        var entities = new List<BodyTypeEntity>
        {
            new() { Name = "Бортовий", Slug = "bortovyi", SortOrder = 1 },
            new() { Name = "Рефрижератор", Slug = "refrizherator", SortOrder = 2 },
            new() { Name = "Тент", Slug = "tent", SortOrder = 3 },
            new() { Name = "Фургон", Slug = "furhon", SortOrder = 4 },
            new() { Name = "Дубль-кабіна борт", Slug = "dubl-kabina-bort", SortOrder = 5 },
            new() { Name = "Самоскид", Slug = "samoskyd", SortOrder = 6 },
            new() { Name = "Евакуатор", Slug = "evakuator", SortOrder = 7 },
            new() { Name = "Платформа", Slug = "platforma", SortOrder = 8 }
        };
        await db.BodyTypes.AddRangeAsync(entities);
        await db.SaveChangesAsync();
    }

    public async Task SeedBrandsAsync()
    {
        if (await db.Brands.AnyAsync())
            return;

        var entities = new List<BrandEntity>
        {
            new () { Name = "Volkswagen" },
            new () { Name = "Mercedes-Benz" },
            new () { Name = "MAN" },
            new () { Name = "Iveco" },
            new () { Name = "Renault" },
            new () { Name = "Ford" },
            new () { Name = "DAF" },
            new () { Name = "Volvo" },
            new () { Name = "Scania" },
            new () { Name = "Peugeot" }
        };
        await db.Brands.AddRangeAsync(entities);
        await db.SaveChangesAsync();
    }

    public async Task SeedVehiclesAsync()
    {
        if (await db.Vehicles.AnyAsync())
            return;

        var brands = await db.Brands.ToDictionaryAsync(x => x.Name);
        var bodies = await db.BodyTypes.ToDictionaryAsync(x => x.Name);
        var fuels = await db.FuelTypes.ToDictionaryAsync(x => x.Name);
        var statuses = await db.VehicleStatuses.ToDictionaryAsync(x => x.Name);

        var now = DateTime.UtcNow;
        var counter = 0;

        VehicleEntity V(
            string brand, string body, string model, string? subtitle,
            int year, int mileage, decimal engine, string fuel, decimal price,
            string description, string status, int daysAgo,
            params (string Name, string Value)[] specs)
        {
            counter++;
            var vehicle = new VehicleEntity
            {
                Brand = brands[brand],
                BodyType = bodies[body],
                Model = model,
                Subtitle = subtitle,
                Year = year,
                Mileage = mileage,
                EngineVolume = engine,
                FuelType = fuels[fuel],
                Price = price,
                Vin = $"SEED{counter:D13}", // 17 символів, умовний VIN
                Description = description,
                Status = statuses[status],
                CreatedAt = now.AddDays(-daysAgo),
                UpdatedAt = daysAgo > 10 ? now.AddDays(-daysAgo + 5) : null
            };

            for (var i = 1; i <= 3; i++)
            {
                vehicle.Images.Add(new VehicleImageEntity
                {
                    Url = $"https://placehold.co/1200x800?text={Uri.EscapeDataString($"{brand} {model} {i}")}",
                    IsMain = i == 1,
                    SortOrder = i
                });
            }

            var order = 1;
            foreach (var (name, value) in specs)
            {
                vehicle.Specifications.Add(new VehicleSpecificationEntity
                {
                    Name = name,
                    Value = value,
                    SortOrder = order++
                });
            }

            return vehicle;
        }

        var vehicles = new List<VehicleEntity>
            {
                V("Volkswagen", "Бортовий", "Crafter", "Комерційний автомобіль",
                    2018, 245_000, 2.0m, "Дизель", 18_500m,
                    "Бортовий Crafter в хорошому стані, обслуговувався у дилера. Дерев'яний борт, без ДТП.",
                    "В наявності", 3,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "140 к.с."),
                    ("Колісна база", "3640 мм"), ("Вантажопідйомність", "1 300 кг"), ("Привід", "Передній")),

                V("Volkswagen", "Фургон", "Crafter", "Цільнометалевий фургон",
                    2019, 198_000, 2.0m, "Дизель", 21_900m,
                    "Цільнометалевий фургон, обшита вантажна частина, дві розсувні двері.",
                    "В наявності", 5,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "177 к.с."),
                    ("Об'єм кузова", "11,3 м³"), ("Колісна база", "3640 мм"), ("Кондиціонер", "Так")),

                V("Mercedes-Benz", "Фургон", "Sprinter 316", "Комерційний автомобіль",
                    2017, 320_000, 2.2m, "Дизель", 17_200m,
                    "Sprinter 316 з Німеччини, свіжа розмальовка не потрібна, технічно справний.",
                    "В наявності", 8,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "163 к.с."),
                    ("Об'єм кузова", "10,5 м³"), ("Екологічний клас", "Euro 6")),

                V("Mercedes-Benz", "Рефрижератор", "Sprinter 314", "Ізотермічний рефрижератор",
                    2016, 410_000, 2.1m, "Дизель", 15_800m,
                    "Рефрижератор з установкою Thermo King, температурний режим від -20 до +20.",
                    "Заброньовано", 14,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "143 к.с."),
                    ("Холодильна установка", "Thermo King V-200"), ("Об'єм кузова", "14 м³")),

                V("Mercedes-Benz", "Дубль-кабіна борт", "Vario 815", "Дубль-кабіна, 7 місць",
                    2008, 520_000, 4.3m, "Дизель", 14_300m,
                    "Дубль-кабіна на 7 осіб, дерев'яна платформа 4,2 м, ідеально для бригад.",
                    "В наявності", 20,
                    ("Коробка передач", "Механіка, 5 ст."), ("Потужність", "150 к.с."),
                    ("Довжина платформи", "4 200 мм"), ("Кількість місць", "7")),

                V("MAN", "Тент", "TGL 12.250", "Тентований вантажний",
                    2015, 480_000, 6.9m, "Дизель", 24_500m,
                    "Тентований MAN TGL, підвіска пневмо, кузов 7,2 м, сідлові ТО пройдені.",
                    "В наявності", 11,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "250 к.с."),
                    ("Довжина кузова", "7 200 мм"), ("Вантажопідйомність", "6 000 кг")),

                V("MAN", "Самоскид", "TGS 33.400", "Самоскид 6x4",
                    2014, 610_000, 10.5m, "Дизель", 38_900m,
                    "Будівельний самоскид 6x4, металевий кузов 16 м³, посилена рама.",
                    "В наявності", 25,
                    ("Коробка передач", "Механіка, 16 ст."), ("Потужність", "400 к.с."),
                    ("Колісна формула", "6x4"), ("Об'єм кузова", "16 м³")),

                V("Iveco", "Бортовий", "Daily 35C13", "Комерційний автомобіль",
                    2016, 360_000, 2.3m, "Дизель", 13_900m,
                    "Iveco Daily здвоєні колеса, борт 4 м, економічний та невибагливий.",
                    "В наявності", 7,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "126 к.с."),
                    ("Довжина платформи", "4 100 мм"), ("Вантажопідйомність", "1 700 кг")),

                V("Iveco", "Евакуатор", "Daily 50C15", "Евакуатор з гідроманіпулятором",
                    2013, 440_000, 3.0m, "Дизель", 22_700m,
                    "Евакуатор з посиленою платформою, лебідка 5 т, повна документація.",
                    "Продано", 40,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "150 к.с."),
                    ("Лебідка", "5 т"), ("Довжина платформи", "5 200 мм")),

                V("Renault", "Фургон", "Master", "Фургон L3H2",
                    2020, 130_000, 2.3m, "Дизель", 20_400m,
                    "Renault Master з невеликим пробігом, один власник, повна сервісна історія.",
                    "В наявності", 2,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "135 к.с."),
                    ("Об'єм кузова", "13 м³"), ("Круїз-контроль", "Так")),

                V("Ford", "Фургон", "Transit", "Цільнометалевий фургон",
                    2018, 275_000, 2.0m, "Дизель", 14_600m,
                    "Ford Transit EcoBlue, мінімальні сліди експлуатації, налаштований під доставку.",
                    "В наявності", 9,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "130 к.с."),
                    ("Об'єм кузова", "9,5 м³")),

                V("Ford", "Платформа", "Transit Chassis", "Шасі-платформа",
                    2019, 205_000, 2.0m, "Дизель", 15_300m,
                    "Шасі Transit з алюмінієвою платформою, підходить для надбудов.",
                    "В наявності", 18,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "170 к.с."),
                    ("Довжина платформи", "4 400 мм")),

                V("DAF", "Тент", "LF 55.250", "Тентований вантажний",
                    2012, 590_000, 6.7m, "Дизель", 19_800m,
                    "DAF LF з тентом 7,3 м, гідроборт 1 т, двигун не вимагає ремонту.",
                    "В наявності", 33,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "250 к.с."),
                    ("Гідроборт", "1 000 кг")),

                V("Volvo", "Рефрижератор", "FL 280", "Рефрижератор 18 палет",
                    2013, 650_000, 7.2m, "Дизель", 29_500m,
                    "Volvo FL з холодильною установкою Carrier, кузов 8 м, підвіска пневмо.",
                    "Заброньовано", 16,
                    ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "280 к.с."),
                    ("Холодильна установка", "Carrier Supra 850"), ("Місткість", "18 євро-палет")),

                V("Scania", "Самоскид", "P 410", "Самоскид 8x4",
                    2016, 540_000, 12.7m, "Дизель", 54_000m,
                    "Scania P410 8x4, кузов Meiller, ретардер, готовий до роботи в кар'єрі.",
                    "В наявності", 12,
                    ("Коробка передач", "Автомат, Opticruise"), ("Потужність", "410 к.с."),
                    ("Колісна формула", "8x4"), ("Ретардер", "Так")),

                V("Peugeot", "Фургон", "e-Boxer", "Електрофургон",
                    2022, 38_000, 1.0m, "Електро", 32_900m,
                    "Електричний фургон для міської доставки, запас ходу до 250 км.",
                    "В наявності", 1,
                    ("Коробка передач", "Автомат"), ("Потужність", "122 к.с."),
                    ("Ємність батареї", "70 кВт·год"), ("Запас ходу", "до 250 км")),

                V("Renault", "Бортовий", "Maxity", "Легкий бортовий",
                    2015, 280_000, 2.5m, "Газ/бензин", 11_200m,
                    "Maxity з ГБО 4 покоління, компактний, зручний для міських перевезень.",
                    "В наявності", 22,
                    ("Коробка передач", "Механіка, 5 ст."), ("Потужність", "130 к.с."),
                    ("ГБО", "4 покоління"), ("Вантажопідйомність", "2 000 кг")),
            };

        db.Vehicles.AddRange(vehicles);
        await db.SaveChangesAsync();

        // ───────────── Заявки ─────────────

        if (!await db.Leads.AnyAsync())
        {
            var leadStatuses = await db.LeadStatuses.ToDictionaryAsync(x => x.Name);

            LeadEntity L(VehicleEntity? vehicle, string name, string phone,
                string? message, string status, int daysAgo) => new()
                {
                    Vehicle = vehicle,
                    Name = name,
                    Phone = phone,
                    Message = message,
                    Status = leadStatuses[status],
                    CreatedAt = now.AddDays(-daysAgo)
                };

            db.Leads.AddRange(
                L(vehicles[0], "Олександр", "+380501234567", "Цікавить можливість огляду завтра.", "Нова", 0),
                L(vehicles[1], "Ірина", "+380671112233", "Чи є можливість лізингу?", "Нова", 1),
                L(vehicles[3], "Максим", "+380931234500", "Хочу забронювати, коли можна приїхати?", "В роботі", 2),
                L(vehicles[5], "Андрій", "+380991234599", null, "В роботі", 3),
                L(vehicles[8], "Віталій", "+380631239876", "Чи актуальне оголошення?", "Відхилена", 15),
                L(vehicles[9], "Наталя", "+380501119988", "Телефонуйте після 15:00.", "Виконана", 6),
                L(vehicles[15], "Сергій", "+380671230000", "Цікавить електрофургон, чи можливий обмін?", "Нова", 0),
                L(null, "Дмитро", "+380731234455", "Шукаю рефрижератор до 20 000 $, що порадите?", "Нова", 1),
                L(null, "Оксана", "+380501010101", null, "Виконана", 10));

            await db.SaveChangesAsync();
        }
    }


    public async Task SeedTestsAsync()
    {
        if (await db.Tests.AnyAsync())
            return;

        var tests = new List<TestEntity>
        {
            new() { Title = CSharpTest, Description = "Базові питання з мови C#", TimeLimitMinutes = 15 },
            new() { Title = EfTest, Description = "Питання про роботу з EF Core", TimeLimitMinutes = 20 }
        };
        await db.Tests.AddRangeAsync(tests);
        await db.SaveChangesAsync();
    }

    public async Task SeedQuestionsAsync()
    {
        if (await db.Questions.AnyAsync())
            return;

        var tests = await db.Tests.ToDictionaryAsync(t => t.Title, t => t.Id);
        if (!tests.ContainsKey(CSharpTest) || !tests.ContainsKey(EfTest))
            return;

        var questions = new List<QuestionEntity>
        {
            new() { Text = QIntType, OrderIndex = 1, Points = 1, TestId = tests[CSharpTest] },
            new() { Text = QAsync, OrderIndex = 2, Points = 1, TestId = tests[CSharpTest] },
            new() { Text = QMigration, OrderIndex = 1, Points = 2, TestId = tests[EfTest] },
            new() { Text = QRelations, OrderIndex = 2, Points = 3, TestId = tests[EfTest] }
        };
        await db.Questions.AddRangeAsync(questions);
        await db.SaveChangesAsync();
    }

    public async Task SeedAnswerOptionsAsync()
    {
        if (await db.AnswerOptions.AnyAsync())
            return;

        var q = await db.Questions.ToDictionaryAsync(x => x.Text, x => x.Id);
        if (new[] { QIntType, QAsync, QMigration, QRelations }.Any(t => !q.ContainsKey(t)))
            return;

        var options = new List<AnswerOptionEntity>
        {
            new() { Text = "int", IsCorrect = true, QuestionId = q[QIntType] },
            new() { Text = "string", IsCorrect = false, QuestionId = q[QIntType] },
            new() { Text = "bool", IsCorrect = false, QuestionId = q[QIntType] },
            new() { Text = "double", IsCorrect = false, QuestionId = q[QIntType] },

            new() { Text = "async", IsCorrect = true, QuestionId = q[QAsync] },
            new() { Text = "await", IsCorrect = false, QuestionId = q[QAsync] },
            new() { Text = "static", IsCorrect = false, QuestionId = q[QAsync] },
            new() { Text = "yield", IsCorrect = false, QuestionId = q[QAsync] },

            new() { Text = "dotnet ef migrations add", IsCorrect = true, QuestionId = q[QMigration] },
            new() { Text = "dotnet ef database update", IsCorrect = false, QuestionId = q[QMigration] },
            new() { Text = "dotnet ef dbcontext scaffold", IsCorrect = false, QuestionId = q[QMigration] },

            new() { Text = "[ForeignKey]", IsCorrect = true, QuestionId = q[QRelations] },
            new() { Text = "[Key]", IsCorrect = true, QuestionId = q[QRelations] },
            new() { Text = "[Route]", IsCorrect = false, QuestionId = q[QRelations] },
            new() { Text = "[HttpGet]", IsCorrect = false, QuestionId = q[QRelations] }
        };
        await db.AnswerOptions.AddRangeAsync(options);
        await db.SaveChangesAsync();
    }

}
