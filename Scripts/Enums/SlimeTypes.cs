using System.ComponentModel.DataAnnotations;

namespace Scripts.Enums
{
    public enum SlimeTypes
    {
        [Display(Name = "Basic Slime")]
        BasicSlime,
        [Display(Name = "Bunny Slime")]
        BunnySlime,
        [Display(Name = "Water Slime")]
        WaterSlime,
        [Display(Name = "Panda Slime")]
        PandaSlime,
        [Display(Name = "Puffer Slime")]
        PufferSlime,
        [Display(Name = "Lava Slime")]
        LavaSlime,
        [Display(Name = "Fox Slime")]
        FoxSlime
    }
}
