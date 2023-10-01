using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HZDZTOUHOU.Items
{
    public class EasyReimuSummoner : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Reimu Summoner");
            //Tooltip.SetDefault("Use to attract boss Reimu(easy)");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 45; // 使用时间，以帧为单位
            Item.useAnimation = 45; // 动画时间，以帧为单位
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(copper:1);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, Mod.Find<ModNPC>("reimu").Type); // 召唤Reimu Boss
            //Main.NewText("Reimu已经被召唤了！", 175, 75, 255); // 发送聊天框消息
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.CopperCoin, 1)
                .Register();
            
        }
    }
}
