using IMSWebPortal.Data.Models.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMSWebPortal.Pages.Dtos.DtoMapping
{
    public class ItemDtoMap
    {
        public ItemDetailModel Map(ItemDetail itemDetail)
        {
            try
            {
                var itemDetailModel = new ItemDetailModel();
                itemDetailModel.Id = itemDetail.Id;
                itemDetailModel.Name = itemDetail.Name;
                itemDetailModel.Sku = itemDetail.Sku;
                itemDetailModel.PrvSku = itemDetail.Sku;
                itemDetailModel.Price = itemDetail.Price;
                itemDetailModel.Qty = itemDetail.Qty;
                itemDetailModel.IsDeleted = itemDetail.IsDeleted;
                return itemDetailModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ItemDetailModel> Map(List<ItemDetail> itemDetails)
        {
            var itemDetailModels = new List<ItemDetailModel>();
            itemDetails.ToList().ForEach(e => itemDetailModels.Add(Map(e)));
            return itemDetailModels;
        }

        public ItemDetail Map(ItemDetailModel itemDetailModel)
        {
            try
            {
                var itemDetail = new ItemDetail();
                itemDetail.Id = itemDetailModel.Id;
                itemDetail.Name = itemDetailModel.Name;
                itemDetail.Sku = itemDetailModel.Sku;
                itemDetail.Price = itemDetailModel.Price;
                itemDetail.Qty = itemDetailModel.Qty;
                itemDetail.IsDeleted = itemDetailModel.IsDeleted;
                return itemDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ItemDetail> Map(List<ItemDetailModel> itemDetailModels)
        {
            var itemDetails = new List<ItemDetail>();
            itemDetailModels.ToList().ForEach(e => itemDetails.Add(Map(e)));
            return itemDetails;
        }
    }
}
