namespace VacinaApi.Tests;

using VacinaApi.Utils;

public class UnitTestCPF
{
  [Fact]
  public void Test1()
  {
    var status = Utils.IsCPFValid("11111111111", out var error_message);

    Console.WriteLine(error_message);
    Assert.False(status);
  }

  [Fact]
  public void Test2()
  {
    var status = Utils.IsCPFValid("22222222222", out var error_message);

    Console.WriteLine(error_message);
    Assert.False(status);
  }

  [Fact]
  public void Test3()
  {
    var status = Utils.IsCPFValid("52998224725", out var error_message);

    Assert.Empty(error_message);
    Assert.True(status);
  }

  [Fact]
  public void Test4()
  {
    var status = Utils.IsCPFValid("43813879100", out var error_message);

    Assert.Empty(error_message);
    Assert.True(status);
  }

  [Fact]
  public void Test5()
  {
    var status = Utils.IsCPFValid("43813879101", out var error_message);

    Console.WriteLine(error_message);
    Assert.False(status);
  }

  [Fact]
  public void Test6()
  {
    var status = Utils.IsCPFValid("43a13879101", out var error_message);

    Console.WriteLine(error_message);
    Assert.False(status);
  }

  [Fact]
  public void Test7()
  {
    var status = Utils.IsCPFValid("413879101", out var error_message);

    Console.WriteLine(error_message);
    Assert.False(status);
  }

}
