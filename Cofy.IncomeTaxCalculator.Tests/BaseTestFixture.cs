[assembly: LevelOfParallelism(100)]

namespace Cofy.IncomeTaxCalculator.Tests;

/// <summary>
/// Base class for test fixtures used for configuring parallelization and test fixture's lifecycle in a single place.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class BaseTestFixture;
