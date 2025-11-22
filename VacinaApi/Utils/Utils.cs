namespace VacinaApi.Utils;

public static class Utils
{
  public static bool IsCPFValid(string cpf, out string error_message)
  {
    if (cpf.Length != 11)
    {
      error_message = "CPF must have 11 digits";
      return false;
    }

    foreach (var digit in cpf)
    {
      if (!int.TryParse(digit.ToString(), out int parsedDigit))
      {
        error_message = "CPF must contain only numbers";
        return false;
      }
    }

    if (cpf.Distinct().Count() <= 1)
    {
      error_message = "Invalid CPF";
      return false;
    }

    var rightDigits = cpf[9..].Select(digit => int.Parse(digit.ToString())).ToArray();

    var leftSum1 = cpf[..9].Select((stringDigit, index) =>
    {
      var digit = int.Parse(stringDigit.ToString());
      var i = 10 - index;

      return digit * i;
    }).Sum();

    if (rightDigits[0] != (leftSum1 * 10 % 11 % 10))
    {
      error_message = "Invalid CPF";
      return false;
    }

    var leftSum2 = cpf[..10].Select((stringDigit, index) =>
    {
      var digit = int.Parse(stringDigit.ToString());
      var i = 11 - index;

      return digit * i;
    }).Sum();

    if (rightDigits[1] != (leftSum2 * 10 % 11 % 10))
    {
      error_message = "Invalid CPF";
      return false;
    }

    error_message = "";
    return true;
  }
}

