using Microsoft.Extensions.DependencyInjection;
using PosApp.Application.Export;
using PosApp.Application.Services;
using PosApp.Domain.Creation;
using PosApp.Domain.Repositories;
using PosApp.Domain.Services;
using PosApp.Domain.Strategies;
using PosApp.Infrastructure;
using PosApp.Infrastructure.Export;
using PosApp.Presentation;

var isDemoMode = args.Contains("--demo", StringComparer.OrdinalIgnoreCase);
var storagePath = isDemoMode ? Path.Combine("demo_output", "demo_pos_data.json") : "pos_data.json";

if (isDemoMode && File.Exists(storagePath))
    File.Delete(storagePath);

var services = new ServiceCollection();

// Infrastructure
services.AddSingleton<IDataStorage>(_ => new JsonStorage(storagePath));
services.AddSingleton<IStudentRepository, JsonStudentRepository>();
services.AddSingleton<IRubricRepository, JsonRubricRepository>();
services.AddSingleton<IGradebookRepository, JsonGradebookRepository>();
services.AddSingleton<IStatementExporter, CsvStatementExporter>();
services.AddSingleton<IStatementExporter, JsonStatementExporter>();

// Domain services, factories & strategies
services.AddSingleton(TimeProvider.System);
services.AddSingleton<IEvaluationFactory, EvaluationFactory>();
services.AddSingleton<IReadOnlyDictionary<string, IGradingStrategy>>(_ =>
    new Dictionary<string, IGradingStrategy>(StringComparer.OrdinalIgnoreCase)
    {
        ["normalized"] = new WeightedNormalizedStrategy(10m),
        ["sum"] = new WeightedSumStrategy()
    });
services.AddSingleton<IFinalGradeCalculator>(_ =>
    new FinalGradeCalculationService().WithRounding(decimals: 2));

// Application services
services.AddSingleton<IStudentService, StudentService>();
services.AddSingleton<IRubricService, RubricService>();
services.AddSingleton<IEvaluationService, EvaluationService>();

// Presentation
services.AddSingleton<IPosConsoleFacade, PosConsoleFacade>();
services.AddSingleton<IConsoleUi, ConsoleUi>();

using var serviceProvider = services.BuildServiceProvider();

var consoleUi = serviceProvider.GetRequiredService<IConsoleUi>();
consoleUi.Run(autoDemoOnly: isDemoMode);