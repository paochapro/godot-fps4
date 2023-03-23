static class AnimationPlayerExtensions
{
    public static void PlayOrPass(this AnimationPlayer player, string anim)
    {
        if(!player.HasAnimation(anim))
        {
            //Needs more info
            GD.PrintErr($"No animation \"{anim}\" was found! AnimationPlayer: {player}");
            return;
        }

        player.Play(anim);
    }
}