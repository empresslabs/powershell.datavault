// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using Bogus;
using Empress.Labs.PowerShell.DataVault.Abstractions;
using Empress.Labs.PowerShell.DataVault.Options;
using Empress.Labs.PowerShell.DataVault.UnitTesting.Mock;

namespace Empress.Labs.PowerShell.DataVault.UnitTesting;

[TestFixture]
[TestOf(typeof(Repository<>))]
public sealed class RepositoryTest {
  [OneTimeSetUp]
  public void OneTimeSetUp()
    => DataVaultModule.OnImport(options => options
        .UseFileName("UnitTesting")
        .UseConnectionType(DataVaultOptions.ConnectionType.Synchronous),
      AppContext.BaseDirectory
    );

  [OneTimeTearDown]
  public void OneTimeTearDown()
    => DataVaultModule.GetRepository<MockEntity>().Truncate();

  public Faker Faker { get; } = new();

  [Test]
  public void Add_WithEntity_ShouldAddEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };

    // Act
    repository.Add(entity);
    var result = repository.Get(entity.Id);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(entity.Name));
  }

  [Test]
  public void Add_WithMultipleEntities_ShouldAddEntities() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    repository.Add(entities);
    var result = repository.Find(entity => identifiers.Contains(entity.Id)).ToArray();

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Not.Empty);
    Assert.That(result.Count, Is.EqualTo(entities.Count));
  }

  [Test]
  public void Update_WithEntity_ShouldUpdateEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };

    // Act
    repository.Add(entity);
    entity.Name = Faker.Name.FullName();
    repository.Update(entity);
    var result = repository.Get(entity.Id);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(entity.Name));
  }

  [Test]
  public void Update_WithMultipleEntities_ShouldUpdateEntities() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var fakeName = Faker.Name.FullName();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => fakeName)
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    repository.Add(entities);
    entities.ForEach(entity => entity.Name = Faker.Name.FullName());
    repository.Update(entities);
    var result = repository.Find(entity => identifiers.Contains(entity.Id)).ToArray();

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Not.Empty);
    Assert.That(result.Count, Is.EqualTo(entities.Count));
    Assert.That(result.All(entity => entity.Name != fakeName), Is.True);
  }

  [Test]
  public void Update_WithPredicate_ShouldUpdateEntities() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var fakeName = Faker.Name.FullName();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => fakeName)
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    repository.Add(entities);
    repository.Update(entity => identifiers.Contains(entity.Id), entity => entity.Name = Faker.Name.FullName());
    var result = repository.Find(entity => identifiers.Contains(entity.Id)).ToArray();

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Not.Empty);
    Assert.That(result.Count, Is.EqualTo(entities.Count));
    Assert.That(result.All(entity => entity.Name != fakeName), Is.True);
  }

  [Test]
  public void Update_WithGuid_ShouldUpdateEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    var newName = Faker.Name.FullName();

    // Act
    repository.Add(entity);
    repository.Update(entity.Id, testEntity => testEntity.Name = newName);
    var result = repository.Get(entity.Id);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(newName));
  }

  [Test]
  public void Delete_WithEntity_ShouldDeleteEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };

    // Act
    repository.Add(entity);
    repository.Delete(entity);
    var result = repository.Get(entity.Id);

    // Assert
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Delete_WithMultipleEntities_ShouldDeleteEntities() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    repository.Add(entities);
    repository.Delete(entities);
    var result = repository.Find(entity => identifiers.Contains(entity.Id)).ToArray();

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Empty);
  }

  [Test]
  public void Delete_WithPredicate_ShouldDeleteEntities() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id);

    // Act
    repository.Add(entities);
    repository.Delete(entity => identifiers.Contains(entity.Id));
    var result = repository.Find(entity => identifiers.Contains(entity.Id)).ToArray();

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Empty);
  }

  [Test]
  public void Delete_WithGuid_ShouldDeleteEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };

    // Act
    repository.Add(entity);
    repository.Delete(entity.Id);
    var result = repository.Get(entity.Id);

    // Assert
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Delete_WithMultipleGuids_ShouldDeleteEntities() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    var identifiers = entities.Select(entity => entity.Id).ToArray();
    var primaryKeys = entities.Select(entity => entity.Id);

    // Act
    repository.Add(entities);
    repository.Delete(identifiers);
    var result = repository.Find(entity => primaryKeys.Contains(entity.Id)).ToArray();

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Empty);
  }

  [Test]
  public void Count_ShouldReturnCorrectCount() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    repository.Truncate();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(5);
    repository.Add(entities);

    // Act
    var count = repository.Count();

    // Assert
    Assert.That(count, Is.EqualTo(5));
  }

  [Test]
  public void Count_WithPredicate_ShouldReturnCorrectCount() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(10);
    repository.Add(entities);

    // Act
    var count = repository.Count(x => x.Name.StartsWith("A"));

    // Assert
    Assert.That(count, Is.GreaterThanOrEqualTo(0));
  }

  [Test]
  public void Exists_ShouldReturnTrueForExistingEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    repository.Add(entity);

    // Act
    var exists = repository.Exists(entity.Id);

    // Assert
    Assert.That(exists, Is.True);
  }

  [Test]
  public void Get_ShouldRetrieveCorrectEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    repository.Add(entity);

    // Act
    var retrievedEntity = repository.Get(entity.Id);

    // Assert
    Assert.That(retrievedEntity, Is.Not.Null);
    Assert.That(retrievedEntity.Name, Is.EqualTo(entity.Name));
  }

  [Test]
  public void Find_ShouldReturnAllEntities() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    repository.Truncate();
    var entities = new Faker<MockEntity>()
      .RuleFor(entity => entity.Name, faker => faker.Name.FullName())
      .Generate(5);
    repository.Add(entities);

    // Act
    var result = repository.Find().ToArray();

    // Assert
    Assert.That(result, Has.Length.EqualTo(5));
  }

  [Test]
  public void Delete_WithEntityId_ShouldDeleteEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    repository.Add(entity);

    // Act
    repository.Delete(entity.Id);
    var result = repository.Get(entity.Id);

    // Assert
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Truncate_ShouldClearTable() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    repository.Add(new Faker<MockEntity>().Generate(1));

    // Act
    repository.Truncate();

    // Assert
    var count = repository.Count();
    Assert.That(count, Is.EqualTo(0));
  }

  [Test]
  public void AsReadOnly_ShouldReturnReadOnlyRepository() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var readOnlyRepo = repository.AsReadOnly();

    // Assert
    Assert.That(readOnlyRepo, Is.Not.Null);
    Assert.That(readOnlyRepo, Is.InstanceOf<IReadOnlyRepository<MockEntity>>());
  }

  [Test]
  public void Get_WithPredicate_ShouldReturnCorrectEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    repository.Add(entity);

    // Act
    var retrievedEntity = repository.Get(x => x.Name == entity.Name);

    // Assert
    Assert.That(retrievedEntity, Is.Not.Null);
    Assert.That(retrievedEntity.Name, Is.EqualTo(entity.Name));
  }

  [Test]
  public void Exists_WithPredicate_ShouldReturnTrueForExistingEntity() {
    // Arrange
    var repository = DataVaultModule.GetRepository<MockEntity>();
    var entity = new MockEntity { Name = Faker.Name.FullName() };
    repository.Add(entity);

    // Act
    var exists = repository.Exists(x => x.Name == entity.Name);

    // Assert
    Assert.That(exists, Is.True);
  }
}
