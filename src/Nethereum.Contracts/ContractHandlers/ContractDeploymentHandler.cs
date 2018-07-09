﻿using System.Threading;
using System.Threading.Tasks;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.DeploymentHandlers;
using Nethereum.Contracts.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionManagers;

namespace Nethereum.Contracts.ContractHandlers
{
#if !DOTNET35
    public class ContractDeploymentTransactionHandler<TContractDeploymentMessage> : ContractTransactionHandlerBase, IContractDeploymentTransactionHandler<TContractDeploymentMessage> where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        private IDeploymentEstimatorHandler<TContractDeploymentMessage> _estimatorHandler;
        private IDeploymentTransactionReceiptPollHandler<TContractDeploymentMessage> _receiptPollHandler;
        private IDeploymentTransactionSenderHandler<TContractDeploymentMessage> _transactionSenderHandler;
        private IDeploymentSigner<TContractDeploymentMessage> _transactionSigner;

        public ContractDeploymentTransactionHandler(IClient client, IAccount account) : base(client, account)
        {
            _estimatorHandler = new DeploymentEstimatorHandler<TContractDeploymentMessage>(client, account);
            _receiptPollHandler = new DeploymentTransactionReceiptPollHandler<TContractDeploymentMessage>(client, account);
            _transactionSenderHandler = new DeploymentTransactionSenderHandler<TContractDeploymentMessage>(client, account);
            _transactionSigner = new DeploymentSigner<TContractDeploymentMessage>(client, account);
        }

        public ContractDeploymentTransactionHandler(ITransactionManager transactionManager) : base(transactionManager)
        {
            _estimatorHandler = new DeploymentEstimatorHandler<TContractDeploymentMessage>(transactionManager);
            _receiptPollHandler = new DeploymentTransactionReceiptPollHandler<TContractDeploymentMessage>(transactionManager);
            _transactionSenderHandler = new DeploymentTransactionSenderHandler<TContractDeploymentMessage>(transactionManager);
            _transactionSigner = new DeploymentSigner<TContractDeploymentMessage>(transactionManager);
        }

        public Task<string> SignTransactionAsync(TContractDeploymentMessage contractDeploymentMessage)
        {
            return _transactionSigner.SignTransactionAsync(contractDeploymentMessage);
        }

        public Task<TransactionReceipt> SendRequestAndWaitForReceiptAsync(
            TContractDeploymentMessage contractDeploymentMessage = null, CancellationTokenSource tokenSource = null)
        {
            return _receiptPollHandler.SendTransactionAsync(contractDeploymentMessage, tokenSource);
        }

        public Task<string> SendRequestAsync(TContractDeploymentMessage contractDeploymentMessage = null)
        {
            return _transactionSenderHandler.SendTransactionAsync(contractDeploymentMessage);
        }

        public Task<HexBigInteger> EstimateGasAsync(TContractDeploymentMessage contractDeploymentMessage)
        {
            return _estimatorHandler.EstimateGasAsync(contractDeploymentMessage);
        }

        public async Task<TransactionInput> CreateTransactionInputEstimatingGasAsync(TContractDeploymentMessage deploymentMessage = null)
        {
            if (deploymentMessage == null) deploymentMessage = new TContractDeploymentMessage();
            var gasEstimate = await EstimateGasAsync(deploymentMessage).ConfigureAwait(false);
            deploymentMessage.Gas = gasEstimate;
            return deploymentMessage.CreateTransactionInput();
        }
    }
#endif

}