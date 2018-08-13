using EdjCase.JsonRpc.Router;
using EdjCase.JsonRpc.Router.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiiiChain.DTO;
using FiiiChain.Framework;
using FiiiChain.Business;
using FiiiChain.Messages;
using FiiiChain.Consensus;
using FiiiChain.Entities;
using Newtonsoft.Json;

namespace FiiiChain.Wallet.API
{
    public class BlockChainController : BaseRpcController
    {
        public IRpcMethodResult StopEngine()
        {
            try
            {
                Startup.EngineStopAction();
                return Ok();
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }
        public IRpcMethodResult GetBlockChainStatus()
        {
            try
            {
                var result = Startup.GetEngineJobStatusFunc();
                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GetBlock(string blockHash, int format = 0)
        {
            try
            {
                var blockComponent = new BlockComponent();

                var block = blockComponent.GetBlockMsgByHash(blockHash);

                if(block != null)
                {
                    if(format == 0)
                    {
                        var bytes = block.Serialize();
                        var result = Base16.Encode(bytes);
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(block);
                    }
                }
                else
                {
                    return Ok();
                }
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GetBlockCount()
        {
            try
            {
                var blockComponent = new BlockComponent();
                var height = blockComponent.GetLatestHeight();
                return Ok(height);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GetBlockHash(long blockHeight)
        {
            try
            {
                var blockComponent = new BlockComponent();
                var block = blockComponent.GetBlockMsgByHeight(blockHeight);

                if(block != null)
                {
                    return Ok(block.Header.Hash);
                }
                else
                {
                    return Ok();
                }
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GetBlockHeader(string blockHash, int format = 0)
        {
            try
            {
                var blockComponent = new BlockComponent();
                var block = blockComponent.GetBlockMsgByHash(blockHash);

                if (block != null)
                {
                    if (format == 0)
                    {
                        var bytes = block.Header.Serialize();
                        var result = Base16.Encode(bytes);
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(block.Header);
                    }
                }
                else
                {
                    return Ok();
                }
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GetChainTips()
        {
            try
            {
                var dict = new BlockComponent().GetChainTips();
                var result = new List<GetChainTipsOM>();

                foreach(var block in dict.Keys)
                {
                    var item = new GetChainTipsOM();
                    item.height = block.Height;
                    item.hash = block.Hash;
                    item.branchLen = dict[block];

                    if(item.branchLen == 0)
                    {
                        item.status = "active";
                    }
                    else
                    {
                        if(block.IsDiscarded)
                        {
                            item.status = "invalid";
                        }
                        else
                        {
                            item.status = "unknown";
                        }
                    }

                    result.Add(item);
                }

                return Ok(result.ToArray());
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GetDifficulty()
        {
            try
            {
                var blockComponent = new BlockComponent();
                var height = blockComponent.GetLatestHeight();
                var newHeight = height + 1;
                var previousBlock = blockComponent.GetBlockMsgByHeight(height);
                var prev4032Block = blockComponent.GetBlockMsgByHeight(newHeight - POW.DiffiucltyAdjustStep);

                long prevBits = 0;

                if(previousBlock != null)
                {
                    prevBits = previousBlock.Header.Bits;
                }

                var work = new POW(newHeight);
                var bits = work.CalculateNextWorkTarget(height, prevBits, prev4032Block);
                var target = work.ConvertBitsToBigInt(bits).ToString("X").PadLeft(64, '0');

                var result = new GetDifficultyOM()
                {
                    height = newHeight,
                    hashTarget = target
                };
                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GenerateNewBlock(string minerName, string address = null, int format = 0)
        {
            try
            {
                var block = new BlockComponent().CreateNewBlock(minerName, address);

                if (block != null)
                {
                    if (format == 0)
                    {
                        var bytes = block.Serialize();
                        var result = Base16.Encode(bytes);
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(block);
                    }
                }
                else
                {
                    return Ok();
                }
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult SubmitBlock(string blockData)
        {
            try
            {
                var bytes = Base16.Decode(blockData);
                var block = new BlockMsg();
                int index = 0;
                try
                {
                    block.Deserialize(bytes, ref index);
                }
                catch
                {
                    throw new CommonException(ErrorCode.Service.BlockChain.BLOCK_DESERIALIZE_FAILED);
                }

                var blockComponent = new BlockComponent();

                var blockInDB = blockComponent.GetBlockMsgByHeight(block.Header.Height);

                if(blockInDB == null)
                {
                    blockComponent.SaveBlockIntoDB(block);
                    Startup.P2PBroadcastBlockHeaderAction(block.Header);
                }
                else
                {
                    throw new CommonException(ErrorCode.Service.BlockChain.SAME_HEIGHT_BLOCK_HAS_BEEN_GENERATED);
                }

                return Ok();
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }
    }
}
