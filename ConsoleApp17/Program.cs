using System;
using System.Collections.Generic;


class Account
{
    public string Number { get; set; }
    public string Owner { get; set; }
    private int balance;

    public int Balance
    {
        get { return balance; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("Баланс не может быть отрицательным.");
            }
            balance = value;
        }
    }

    public string Currency { get; set; }

    public Account(string number, string owner, int balance, string currency)
    {
        Number = number;
        Owner = owner;
        Balance = balance;
        Currency = currency;
    }

    public void Deposit(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException("Сумма депозита должна быть положительной.");
        }
        Balance += amount;
    }

    public void Withdraw(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException("Сумма снятия должна быть положительной.");
        }
        if (amount > Balance)
        {
            throw new InvalidOperationException("Недостаточно средств на счете.");
        }
        Balance -= amount;
    }
}

class Transaction
{
    public string AccountNumber { get; set; }

    public string Type { get; set; }

    public int Amount { get; set; }

    public Transaction(string accountNumber, string type, int amount)
    {
        AccountNumber = accountNumber;
        Type = type;
        Amount = amount;
    }
}

class Bank
{
    public string Name { get; set; }

    public Dictionary<string, Account> Accounts = new Dictionary<string, Account>();

    public List<Transaction> Transactions = new List<Transaction>();

    public Bank(string name)
    {
        Name = name;
    }

    public void AddAccount(Account account)
    {
        if (Accounts.ContainsKey(account.Number))
        {
            throw new InvalidOperationException("Счет с таким номером уже существует.");
        }
        Accounts[account.Number] = account;
    }

    public Account FindAccount(string accountNumber)
    {
        
        foreach (KeyValuePair<string, Account> pair in Accounts)
        {
            if (pair.Key == accountNumber)
            {
                return pair.Value;
            }
        }
        throw new KeyNotFoundException("Счет с таким номером не найден.");
    }

    public void Deposit(string accountNumber, int amount)
    {

        Account account = FindAccount(accountNumber);
        if (account == null)
        {
            throw new KeyNotFoundException("Счет не найден");
        }
        account.Deposit(amount);
        Transactions.Add(new Transaction(accountNumber, "Deposit", amount));
    }

    public void Withdraw(string accountNumber, int amount)
    {
        Account account = FindAccount(accountNumber);
        if (account == null)
        {
            throw new KeyNotFoundException("Счет не найден");
        }
        account.Withdraw(amount);
        Transactions.Add(new Transaction(accountNumber, "Withdraw", amount));
    }
}

class Program
{
    static void Main()
    {
        Bank bank = new Bank("ВТБ");

        while (true)
        {
            try
            {
                Console.WriteLine("Выберите действие:" +
                    "\n1. Создать счет\n2. Внести депозит\n3. Снять средства\n4. Показать отчеты \n5. Выйти");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine("Введите номер счета:");
                    string number = Console.ReadLine();
                    Console.WriteLine("Введите владельца счета:");
                    string owner = Console.ReadLine();
                    Console.WriteLine("Введите начальный баланс:");
                    int balance = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите валюту счета:");
                    string currency = Console.ReadLine();
                    Account account = new Account(number, owner, balance, currency);
                    bank.AddAccount(account);
                    Console.WriteLine("Счет успешно создан.");
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Введите номер счета:");

                    string number = Console.ReadLine();
                    Console.WriteLine("Введите сумму депозита:");
                    int amount = Convert.ToInt32(Console.ReadLine());
                    bank.Deposit(number, amount);
                    Console.WriteLine("Депозит успешно выполнен.");
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Введите номер счета:");
                    string number = Console.ReadLine();
                    Console.WriteLine("Введите сумму снятия:");
                    int amount = Convert.ToInt32(Console.ReadLine());
                    bank.Withdraw(number, amount);
                    Console.WriteLine("Снятие успешно выполнено.");
                }
                else if (choice == "4")
                {
                    foreach (KeyValuePair<string, Account> pair in bank.Accounts)
                    {
                        Console.WriteLine($"Счет: {pair.Key}, Владелец: {pair.Value.Owner}, Баланс: {pair.Value.Balance} {pair.Value.Currency}");
                    }


                    Dictionary<string, int> currencyBalances = new Dictionary<string, int>();

                    foreach (KeyValuePair<string, Account> pair in bank.Accounts)
                    {
                        if (currencyBalances.ContainsKey(pair.Value.Currency))
                        {
                            currencyBalances[pair.Value.Currency] += pair.Value.Balance;
                        }
                        else
                        {
                            currencyBalances[pair.Value.Currency] = pair.Value.Balance;
                        }
                    }

                    Console.WriteLine("Отчет по валютам:");
                    Console.WriteLine();
                    foreach (KeyValuePair<string, int> pair in currencyBalances)
                    {
                        Console.WriteLine($"Валюта: {pair.Key}, Баланс: {pair.Value}");
                    }

                    HashSet<string> uniqueOwners = new HashSet<string>();
                    foreach (KeyValuePair<string, Account> pair in bank.Accounts)
                    {
                        uniqueOwners.Add(pair.Value.Owner);
                    }
                    Console.WriteLine("Уникальные владельцы:");
                    
                    foreach (string owner in uniqueOwners)
                    {
                        Console.WriteLine(owner);
                    }

                    Console.WriteLine();
                    string WinnerOwner = null;
                    int maxBalance = 0;

                    foreach (KeyValuePair<string, Account> pair in bank.Accounts)
                    {
                        if (pair.Value.Balance > maxBalance)
                        {
                            maxBalance = pair.Value.Balance;
                            WinnerOwner = pair.Value.Owner;
                        }
                    }
                    Console.WriteLine($"Победитель: {WinnerOwner}, Максимальный баланс: {maxBalance}");
                    Console.WriteLine();

                    Dictionary<string,int> TransactionsCount = new Dictionary<string, int>();

                    foreach (Transaction transaction in bank.Transactions)
                    {
                        if (TransactionsCount.ContainsKey(transaction.Type))
                        {
                            TransactionsCount[transaction.Type]++;
                        }
                        else
                        {
                            TransactionsCount.Add(transaction.Type, 1);
                        }
                    }
                    Console.WriteLine("Количество Депозитов и Снятий:");
                    foreach (KeyValuePair<string,int> pair in TransactionsCount)
                    {
                        Console.WriteLine($"{pair.Key} количество: {pair.Value}");
                    }
                    Console.WriteLine();

                    int alltransactionamount = 0;

                    if (bank.Transactions.Count != 0)
                    {
                        foreach (Transaction transac in bank.Transactions)
                        {
                            alltransactionamount += transac.Amount;
                        }
                        int averagewithdraw = alltransactionamount / bank.Transactions.Count;
                        Console.WriteLine("Общая сумма транзакций:");
                        Console.WriteLine(averagewithdraw);
                    }
                    Console.WriteLine();

                    Console.WriteLine("Владельцы у которых больше 1 счёта:");
                    Dictionary<string, int> RichOwners = new Dictionary<string, int>();

                    foreach (KeyValuePair<string,Account> pair in bank.Accounts)
                    {
                        if (RichOwners.TryGetValue(pair.Value.Owner, out int value))
                        {
                            RichOwners[pair.Value.Owner] = value+1;
                        }
                        else
                        {
                            RichOwners.Add(pair.Value.Owner, 1);
                        }
                    }
                    foreach (KeyValuePair<string,int> pair in RichOwners)
                    {
                        if (pair.Value>1)
                        {
                            Console.WriteLine(pair.Key);
                        }
                    }



                }
                if (choice == "5")
                {
                    break;
                }

            }
            catch (FormatException)
            {
                Console.WriteLine("Сумма должна быть числом.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Сумма слишком велика.");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Сумма не может быть равна нулю.");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Недопустимое действие.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (Exception)
            {
                Console.WriteLine($"Что то пошло не так.");
            }
            finally
            {
                Console.WriteLine("-----BANK-----");
            }
        }
    }
}
