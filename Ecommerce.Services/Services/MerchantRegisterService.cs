using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;
using Stripe;
using Stripe.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class MerchantRegisterService
{
    private readonly string _stripeSecretKey;
    private readonly AccountService _accountService;
    private readonly FileService _fileService;
    private readonly BankAccountService _bankAccountService;
    private readonly VerificationSessionService _verificationSessionService;

    public MerchantRegisterService()
    {
        _stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") 
                           ?? throw new InvalidOperationException("STRIPE_SECRET_KEY is not set.");
        StripeConfiguration.ApiKey = _stripeSecretKey;

        _accountService = new AccountService();
        _fileService = new FileService();
        _bankAccountService = new BankAccountService();
        _verificationSessionService = new VerificationSessionService();
    }

    /// <summary>
    /// Enregistre un marchand avec Stripe et retourne le statut de l'enregistrement.
    /// </summary>
    public async Task<PaymentResponse> RegisterMerchantAsync(MerchantRegisterDTO userDTO)
    {
        // Étape 1 : Créer le compte Stripe
        var accountService = new AccountService();
        var accountOptions = new AccountCreateOptions
        {
            Type = "custom",
            Country = userDTO.Country,
            Email = userDTO.Email,
            BusinessType = "individual", // ou "company" selon vos besoins
            Capabilities = new AccountCapabilitiesOptions
            {
                CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
            }
        };
        var account = await accountService.CreateAsync(accountOptions);

        // Étape 2 : Ajouter un compte bancaire
        await AddBankAccountAsync(account.Id, userDTO);

        // Étape 3 : Retourner la réponse
        return new PaymentResponse
        {
            Status = PaymentStatus.Completed,
            TransactionId = account.Id,
            ErrorMessage = null
        };
    }

    /// <summary>
    /// Télécharge un document sur Stripe et retourne son ID.
    /// </summary>
    private async Task<string> UploadDocumentAsync(DocumentDTO document)
    {
        using var memoryStream = new MemoryStream(document.FileContent);
        var fileOptions = new FileCreateOptions
        {
            File = memoryStream,
            Purpose = "identity_document"
        };

        var file = await _fileService.CreateAsync(fileOptions);
        return file.Id;
    }

    /// <summary>
    /// Crée un compte connecté Stripe pour le marchand.
    /// </summary>
    private async Task<string> CreateStripeAccountAsync(MerchantRegisterDTO userDTO)
    {
        var accountOptions = new AccountCreateOptions
        {
            Type = "custom",
            Country = userDTO.Country,
            Email = userDTO.Email,
            Capabilities = new AccountCapabilitiesOptions
            {
                Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
            },
            BusinessType = "individual",
            Individual = new AccountIndividualOptions
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
            }
        };

        var account = await _accountService.CreateAsync(accountOptions);
        return account.Id;
    }

    /// <summary>
    /// Ajoute un compte bancaire au compte connecté Stripe.
    /// </summary>
    private async Task AddBankAccountAsync(string accountId, MerchantRegisterDTO userDTO)
    {
        // Étape 1 : Créer un token bancaire
        var tokenService = new TokenService();
        var tokenOptions = new TokenCreateOptions
        {
            BankAccount = new TokenBankAccountOptions
            {
                Country = userDTO.Country,
                Currency = userDTO.Currency.ToString().ToLower(),
                AccountHolderName = userDTO.AccountHolderName,
                AccountHolderType = userDTO.AccountHolderType, // "individual" ou "company"
                RoutingNumber = userDTO.RoutingNumber,
                AccountNumber = userDTO.IBAN,
            }
        };
        var bankAccountToken = await tokenService.CreateAsync(tokenOptions);

        // Étape 2 : Associer le token bancaire au compte Stripe
        var accountService = new AccountService();
        var accountUpdateOptions = new AccountUpdateOptions
        {
            ExternalAccount = bankAccountToken.Id 
        };

        await accountService.UpdateAsync(accountId, accountUpdateOptions);
    }


    /// <summary>
    /// Crée une session de vérification sur Stripe.
    /// </summary>
    private async Task<VerificationSession> CreateVerificationSessionAsync(Guid userId, string accountId, string documentFileId)
    {
        var options = new VerificationSessionCreateOptions
        {
            Type = "document",
            Metadata = new Dictionary<string, string>
            {
                { "user_id", userId.ToString() },
                { "account_id", accountId }
            }
        };

        if (!string.IsNullOrEmpty(documentFileId))
        {
            options.Metadata.Add("document_file_id", documentFileId);
        }

        return await _verificationSessionService.CreateAsync(options);
    }
}
