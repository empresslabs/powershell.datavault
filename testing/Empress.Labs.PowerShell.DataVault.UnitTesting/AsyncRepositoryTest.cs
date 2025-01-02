// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using Bogus;
using Empress.Labs.PowerShell.DataVault.Abstractions;
using Empress.Labs.PowerShell.DataVault.Options;
using Empress.Labs.PowerShell.DataVault.UnitTesting.Mock;

namespace Empress.Labs.PowerShell.DataVault.UnitTesting;

[TestFixture]
[TestOf(typeof(AsyncRepository<>))]
public sealed class AsyncRepositoryTest {
  [OneTimeSetUp]
  public void OneTimeSetUp()
    => DataVaultModule.OnImport(options => options
        .UseFileName("UnitTesting")
        .UseConnectionType(DataVaultOptions.ConnectionType.Asynchronous),
      AppContext.BaseDirectory
    );

  [OneTimeTearDown]
  public Task OneTimeTearDown()
    => DataVaultModule.GetAsyncRepository<MockEntity>().TruncateAsync();

  public Faker Faker { get; } = new();

  [Test]
  public async Task Add_WithEntity_ShouldAddEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };

    // Act
    await repository.AddAsync(entity);
    var result = await repository.GetAsync(entity.Id);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(entity.Name));
  }

  [Test]
  public async Task Add_WithMultipleEntities_ShouldAddEntities() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    await repository.AddAsync(entities);
    var result = await repository.FindAsync(entity => identifiers.Contains(entity.Id));

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Not.Empty);
    Assert.That(result, Has.Count.EqualTo(entities.Count));
  }

  [Test]
  public async Task Update_WithEntity_ShouldUpdateEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };

    // Act
    await repository.AddAsync(entity);
    entity.Name = Faker.Name.FullName();
    await repository.UpdateAsync(entity);
    var result = await repository.GetAsync(entity.Id);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(entity.Name));
  }

  [Test]
  public async Task Update_WithMultipleEntities_ShouldUpdateEntities() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var fakeName = Faker.Name.FullName();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => fakeName)
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    await repository.AddAsync(entities);
    entities.ForEach(entity => entity.Name = Faker.Name.FullName());
    await repository.UpdateAsync(entities);
    var result = await repository.FindAsync(entity => identifiers.Contains(entity.Id));

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Not.Empty);
    Assert.That(result, Has.Count.EqualTo(entities.Count));
    Assert.That(result.All(entity => entity.Name != fakeName), Is.True);
  }

  [Test]
  public async Task Update_WithPredicate_ShouldUpdateEntities() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var fakeName = Faker.Name.FullName();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => fakeName)
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    await repository.AddAsync(entities);
    await repository.UpdateAsync(entity => identifiers.Contains(entity.Id), entity => entity.Name = Faker.Name.FullName());
    var result = await repository.FindAsync(entity => identifiers.Contains(entity.Id));

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Not.Empty);
    Assert.That(result, Has.Count.EqualTo(entities.Count));
    Assert.That(result.All(entity => entity.Name != fakeName), Is.True);
  }

  [Test]
  public async Task Update_WithGuid_ShouldUpdateEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    var newName = Faker.Name.FullName();

    // Act
    await repository.AddAsync(entity);
    await repository.UpdateAsync(entity.Id, testEntity => testEntity.Name = newName);
    var result = await repository.GetAsync(entity.Id);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(newName));
  }

  [Test]
  public async Task Delete_WithEntity_ShouldDeleteEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };

    // Act
    await repository.AddAsync(entity);
    await repository.DeleteAsync(entity);
    var result = await repository.GetAsync(entity.Id);

    // Assert
    Assert.That(result, Is.Null);
  }

  [Test]
  public async Task Delete_WithMultipleEntities_ShouldDeleteEntities() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    await repository.AddAsync(entities);
    await repository.DeleteAsync(entities);
    var result = await repository.FindAsync(entity => identifiers.Contains(entity.Id));

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Empty);
  }

  [Test]
  public async Task Delete_WithPredicate_ShouldDeleteEntities() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    await repository.AddAsync(entities);
    await repository.DeleteAsync(entity => identifiers.Contains(entity.Id));
    var result = await repository.FindAsync(entity => identifiers.Contains(entity.Id));

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Empty);
  }

  [Test]
  public async Task Delete_WithGuid_ShouldDeleteEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };

    // Act
    await repository.AddAsync(entity);
    await repository.DeleteAsync(entity.Id);
    var result = await repository.GetAsync(entity.Id);

    // Assert
    Assert.That(result, Is.Null);
  }

  [Test]
  public async Task Delete_WithMultipleGuids_ShouldDeleteEntities() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id).ToArray();
    var primaryKeys = entities.Select(entity => entity.Id);

    // Act
    await repository.AddAsync(entities);
    await repository.DeleteAsync(identifiers);
    var result = await repository.FindAsync(entity => primaryKeys.Contains(entity.Id));

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Empty);
  }

  [Test]
  public async Task Count_ShouldReturnCorrectCount() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    await repository.TruncateAsync();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(5);
    await repository.AddAsync(entities);

    // Act
    var count = await repository.CountAsync();

    // Assert
    Assert.That(count, Is.EqualTo(5));
  }

  [Test]
  public async Task Count_WithPredicate_ShouldReturnCorrectCount() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    await repository.AddAsync(entities);

    // Act
    var count = await repository.CountAsync(x => x.Name.StartsWith("A"));

    // Assert
    Assert.That(count, Is.GreaterThanOrEqualTo(0));
  }

  [Test]
  public async Task Exists_ShouldReturnTrueForExistingEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    await repository.AddAsync(entity);

    // Act
    var exists = await repository.ExistsAsync(entity.Id);

    // Assert
    Assert.That(exists, Is.True);
  }

  [Test]
  public async Task Get_ShouldRetrieveCorrectEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    await repository.AddAsync(entity);

    // Act
    var retrievedEntity = await repository.GetAsync(entity.Id);

    // Assert
    Assert.That(retrievedEntity, Is.Not.Null);
    Assert.That(retrievedEntity.Name, Is.EqualTo(entity.Name));
  }

  [Test]
  public async Task Find_ShouldReturnAllEntities() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    await repository.TruncateAsync();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(5);
    await repository.AddAsync(entities);

    // Act
    var result = await repository.FindAsync();

    // Assert
    Assert.That(result, Has.Count.EqualTo(5));
  }

  [Test]
  public async Task Delete_WithEntityId_ShouldDeleteEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    await repository.AddAsync(entity);

    // Act
    await repository.DeleteAsync(entity.Id);
    var result = await repository.GetAsync(entity.Id);

    // Assert
    Assert.That(result, Is.Null);
  }

  [Test]
  public async Task Truncate_ShouldClearTable() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    await repository.AddAsync(new Faker<MockEntity>().Generate(1));

    // Act
    await repository.TruncateAsync();

    // Assert
    var count = await repository.CountAsync();
    Assert.That(count, Is.EqualTo(0));
  }

  [Test]
  public void AsReadOnly_ShouldReturnReadOnlyRepository() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var readOnlyRepo = repository.AsReadOnly();

    // Assert
    Assert.That(readOnlyRepo, Is.Not.Null);
    Assert.That(readOnlyRepo, Is.InstanceOf<IReadOnlyAsyncRepository<MockEntity>>());
  }

  [Test]
  public async Task Get_WithPredicate_ShouldReturnCorrectEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    await repository.AddAsync(entity);

    // Act
    var retrievedEntity = await repository.GetAsync(x => x.Name == entity.Name);

    // Assert
    Assert.That(retrievedEntity, Is.Not.Null);
    Assert.That(retrievedEntity.Name, Is.EqualTo(entity.Name));
  }

  [Test]
  public async Task Exists_WithPredicate_ShouldReturnTrueForExistingEntity() {
    // Arrange
    var repository = DataVaultModule.GetAsyncRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    await repository.AddAsync(entity);

    // Act
    var exists = await repository.ExistsAsync(x => x.Name == entity.Name);

    // Assert
    Assert.That(exists, Is.True);
  }
}
