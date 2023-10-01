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
            Item.useTime = 45; // ʹ��ʱ�䣬��֡Ϊ��λ
            Item.useAnimation = 45; // ����ʱ�䣬��֡Ϊ��λ
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(copper:1);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, Mod.Find<ModNPC>("reimu").Type); // �ٻ�Reimu Boss
            //Main.NewText("Reimu�Ѿ����ٻ��ˣ�", 175, 75, 255); // �����������Ϣ
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
