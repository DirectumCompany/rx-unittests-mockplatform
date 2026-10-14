using MockPlatform.Repositories;
using Sungero.Domain.Shared;

namespace MockPlatform.Tests;

public class Collection
{
  [Test]
  public void CollectionProperyIsNotNull()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();
    Assert.IsNotNull(box.ExchangeServiceCertificates);
  }

  [Test]
  public void CollectionProperyAddNew()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();
    var row = box.ExchangeServiceCertificates.AddNew();
    Assert.IsNotNull(row);
    Assert.AreEqual(1, box.ExchangeServiceCertificates.Count);
  }

  [Test]
  public void CollectionAddMultipleItems()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();

    var row1 = box.ExchangeServiceCertificates.AddNew();
    var row2 = box.ExchangeServiceCertificates.AddNew();
    var row3 = box.ExchangeServiceCertificates.AddNew();

    Assert.AreEqual(3, box.ExchangeServiceCertificates.Count);
    Assert.IsNotNull(row1);
    Assert.IsNotNull(row2);
    Assert.IsNotNull(row3);
  }

  [Test]
  public void CollectionRemoveItem()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();

    var row1 = box.ExchangeServiceCertificates.AddNew();
    var row2 = box.ExchangeServiceCertificates.AddNew();
    var row3 = box.ExchangeServiceCertificates.AddNew();

    Assert.AreEqual(3, box.ExchangeServiceCertificates.Count);

    box.ExchangeServiceCertificates.Remove(row2);

    Assert.AreEqual(2, box.ExchangeServiceCertificates.Count);
  }

  [Test]
  public void CollectionClear()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();

    box.ExchangeServiceCertificates.AddNew();
    box.ExchangeServiceCertificates.AddNew();
    box.ExchangeServiceCertificates.AddNew();

    Assert.AreEqual(3, box.ExchangeServiceCertificates.Count);

    box.ExchangeServiceCertificates.Clear();

    Assert.AreEqual(0, box.ExchangeServiceCertificates.Count);
  }

  [Test]
  public void CollectionLinqFirstOrDefault()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();

    var row1 = box.ExchangeServiceCertificates.AddNew();

    var row2 = box.ExchangeServiceCertificates.AddNew();

    var row3 = box.ExchangeServiceCertificates.AddNew();

    var found = box.ExchangeServiceCertificates.FirstOrDefault(r => r.Id == 2);

    Assert.IsNotNull(found);
    Assert.AreEqual(2, found.Id);
    Assert.AreSame(row2, found);
  }

  [Test]
  public void CollectionLinqWhere()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();

    var row1 = box.ExchangeServiceCertificates.AddNew();
    var row2 = box.ExchangeServiceCertificates.AddNew();
    var row3 = box.ExchangeServiceCertificates.AddNew();

    var filtered = box.ExchangeServiceCertificates.Where(r => r.Id > 1).ToList();

    Assert.AreEqual(2, filtered.Count);
    Assert.IsTrue(filtered.Any(r => r.Id == 2));
    Assert.IsTrue(filtered.Any(r => r.Id == 3));
  }

  [Test]
  public void CollectionLinqAny()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();

    Assert.IsFalse(box.ExchangeServiceCertificates.Any());

    var row = box.ExchangeServiceCertificates.AddNew();

    Assert.IsTrue(box.ExchangeServiceCertificates.Any());
    Assert.IsTrue(box.ExchangeServiceCertificates.Any(r => r.Id == 1));
    Assert.IsFalse(box.ExchangeServiceCertificates.Any(r => r.Id == 999));
  }

  [Test]
  public void CollectionLinqCount()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();

    box.ExchangeServiceCertificates.AddNew();
    box.ExchangeServiceCertificates.AddNew();

    var row3 = box.ExchangeServiceCertificates.AddNew();
    ((IExtendedEntity)row3).Id = 100;

    Assert.AreEqual(3, box.ExchangeServiceCertificates.Count());
    Assert.AreEqual(1, box.ExchangeServiceCertificates.Count(r => r.Id == 100));
    Assert.AreEqual(2, box.ExchangeServiceCertificates.Count(r => r.Id != 100));
  }

  [Test]
  public void CollectionEnumeration()
  {
    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();

    for (var i = 1; i <= 5; i++)
    {
      var row = box.ExchangeServiceCertificates.AddNew();
    }

    long sum = 0;
    foreach (var item in box.ExchangeServiceCertificates)
    {
      sum += item.Id;
    }

    Assert.AreEqual(15, sum); // 1+2+3+4+5 = 15
  }
}
