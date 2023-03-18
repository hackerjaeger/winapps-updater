﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "112.0b3";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b3/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "7c5f17ed9d7549f8a8a32c73a72a0861adef8e72d1cdfb1d6cc1137518948bea01a97a57517ef8f9b94c8b2ca47283b4db4affefe2e126c106dfbcf26368698a" },
                { "af", "98e921f79793107a5beb24c2ae50fb60b6f31ee0989a815b89a8921bc072e73d5e672a2775e5e16b8c0daa8893fd4dcca681850478a9a288b196e61cc6062868" },
                { "an", "3469467bc3c7af9449453e8c88b8d550401247dcae36afa0cdf9c99fe5969e639cc08cca76744f39473d0cdc2941ad83c51c595e15e932e2784a82a28fc5a6a9" },
                { "ar", "b67d464419340a80a3f8815208396b5564f224f730115cf5f169110274dc5a22a13fd8b4683d922e56a78108016a38eb95b6c644bf29cd26b74dca5fcbd127a5" },
                { "ast", "b5f9ce61815f6cc5bd445c2862cbbb0d2b726fdaaabf35705ec84edd775863568c18cc0d9e51940eab68e30830a4bd5f6dbe00b8fe108a873872b2b0d7dc0e74" },
                { "az", "cedf9f377d8ce124fc918edf0e2c66b330d5d6903168a965ed205c726cd7e6997c64f873ede9e6da86536396db63c679a034f1b9862f5742f163358209f430e7" },
                { "be", "a0d6919cd416cd31a95cfc4526a17abeb1669cb192ca72b4e793f2cb2b1c596fb7d1396afdbec779dfe7c2b1afa6bd5297f045f40f130e56de416a332305261e" },
                { "bg", "ff2900c067673f1b6a5fce70212c954948e1c9e16a480a8f5cc890ce13eb173d0f031587078e4ccc0631a4247d733d56c9da88ae08468be451024b3e7490467a" },
                { "bn", "b9caff8136e9df71e1ee6625661312c6647ade514fac0487f13aa520e9557c324e2e34d9e58a66e0f369e573da97ee001e0e3a0afb85510ff2f0479aa7bc48fc" },
                { "br", "2c2ea9a2c05410729709ecf09ec3d108a1bfa0fa8a916f037f6309d3f5afd13642c6f11765fa29c8092364e5b2146bc63840a69b62686539eee199f557cb9cfd" },
                { "bs", "d3f43c781ffef028304e46a4671e052565a49f757f6955d21c5b914871a64c2726cb4b3557e84a31c443c7a72af7985d161f648a6af5d6c99f1a5db61315db0c" },
                { "ca", "332dc1427f3d8abe87c354344d0923a75c310079a0fde14ecc735ea5cf46c044d850d9d05e5ababdf3fe08a29167020f61d80800815b0c904bc027a5fa60c34c" },
                { "cak", "26f537001229f897f9efbbaad3594220fd4aaca9fa829ab52a6d554f4c7f94471b8327c06018215fc7f6c0290b7e66095b72fa483859f61e7b91718294dffbf0" },
                { "cs", "5d82420a02e89ea3217bf19391d8c463d2673289238c9152cc5d83018c68cfe17794381c4ac45c9539567a65194b2742d9465ea8f68b8ca0f772feb33cf6c6e4" },
                { "cy", "ed49055f7ec2719e47deba5a7b2eb07e9d3bd6b7ad8158bee316476d9b8dafe3c9c25995a75ce1674b761bb9beb30b795f021e149473d232693522c525db30fb" },
                { "da", "08d68d174e8401b5146cb1fe919e973f598b2051c3493cf2c78d3fc6bc6142aa9e07e40b59035b30429eb0ff3ce159528687cbb5460eb9604450ddc392c8aea3" },
                { "de", "ce3ad7dba47827ba9c09b697c00ea0d0c4733d338bd820394d5fc9f585ea2ab03eba16ce90a836e4e83edc7e92d087cc94f19adafe81c31cc47f1bce35484ed6" },
                { "dsb", "ab2b778fdc6b717104efdc5b1f84ca5e9e721a87a37650444066f4186ebf6023b2d92ba7ec4ea0ee2e76423faf6a1a28999ceec67a08ee13a194bcd3442e10ba" },
                { "el", "c206f889c88e8ef4c69dc50fbe909c822baa1256c857a4f0f186a3cd61d2c4a761de13e328191d51612f4b00a9478d748adea685042e27a26ecdbfc51b93fc88" },
                { "en-CA", "098fdef7545bed34379d32b490f4213af07a55cd9cfa32d4e3ed13de7bf3656d242ddec8f4bc0fcd3d982c9d6244febea34df5f1d17de916260bf35c6e247f42" },
                { "en-GB", "a91af76a91dc29aca6b4936d8ac8bd3ab5a3968d3c43d8ce26d105b27c028dec3f9f739320fc411c88726524c0ce4655773935c7b69b95d33fc41e23cad51fdb" },
                { "en-US", "19ef531cccf510c8c406610b8695f30b52b14e33eed34e8450afe47f4fe659162117e76cd48b7bf1d3305b91c0b799f9196688a54e4e267ea58e8296ff17f174" },
                { "eo", "38267378531addf32eba18d182ee17a8ef1b9da848c8bbef17ebf05c11043ea279207bb701e65cafa5a605b8c7feb8bde1bbd6c270b43a3c976da6d0d11b10f3" },
                { "es-AR", "331b85a8314edaa7ed0176a731243f130cfbac8b16c8608ae03c5533906b28a65d83ceec9f226995f89efa16f4140cc2b189ad5053d8d328c7e89f951943abd5" },
                { "es-CL", "0cdafd726301b896f42d71643472ee68d8b2cfb9099089b6fe75005bce16aff5122083394bbfa19f254396d77da3e27e7010959673af82f8e7567e0e37d23238" },
                { "es-ES", "652fe15374fe09ca0cc959ed8e585335baddbba1854bf0c36cd4cdff30c28b240ae71322d4f92544ed40778fc0ae019d8d640292c5e0731955009fb7e4dfedf1" },
                { "es-MX", "50b10290f16b8ac53a20d1889cd3c31fbcf5579d0c637a8cd4eb755ebfa16350235c9a7b66c0dd8d6f9774b9336b2436737f01caba5d63436c66835cff9d9a2c" },
                { "et", "73e76f03ec5b8d0ff5043a491b7b4e75a7b556e8db0e40b185b05ffef10c67f38a0f5eddfb6b93a0f4fbfef0f87eb815200d6d4a90a0f2fe194ac039dc40195f" },
                { "eu", "4aeda8c0ddbecfc786356d4f5d197a3156d7330efe3096d7d9a2d1ea412c1b0ad44dfc7dd50a38a67ac55d2c9896b6e1a66262580989a2cfad337f2bc523c707" },
                { "fa", "efe9319327cb2a61ebb64759cb0d4cf0cd4f04554c6b46e82499bc2568527f2c09ade321cbdc84c5c2152fecd73cdf658c0fa4194d48eaee1f2fb7f359fda136" },
                { "ff", "8b4131fc4a49d5ca48c8201ac0877419b25fa820a925fdc6798fd816275e1c780becc9572915c27bd183047932b071403a0d82c6fa58f7cd17643835fbdb40d3" },
                { "fi", "cdf937bec39b2272c6be157d60d2e2cee3b80fe9556bc59a2b3ac5ade5f301123d019a8894c3f87a298fca991da0c4aefe3a6863d682b466131c6a4cb7e078e1" },
                { "fr", "3e051b6a0d217576affeabebfeca407112f99913344606b570111ed4faa27f5e9cb409a137d356bbd416ef143decede49225c091527cbcc7041bfc1e3971e2cd" },
                { "fur", "dda9c26eeb4609189a7998d1603209355655c816f06fc50f9f2148ed3d0c93d22fda9ce62e5f52d46a730c443094319c4626e8810db30d454f27dc504f205ecd" },
                { "fy-NL", "a907efec7c0836b26cd4247ccacc9c6fe46e290cea4dea5ed4bc984d0ea7b49fd13cf9a27981c63c06f68eb692e2a8426e3dcf175a2c3cdd45b3ed25438cbbd3" },
                { "ga-IE", "6dc7c281d0f5bed83c3866c4cca064762d8a40ebbfb95c0e21ed191404687caeeb5ccdb1560a8ea61a7b2a323850a81dc6241bef26ca124a8c6904caec59f62f" },
                { "gd", "a7a82071019a2ddc7deea121c2287e3f3277d5f0ca4e18cace47dc556918808808a3933b1d99aa4f8ce7103898e81a62517e86f3700eecbdffd56d392cc0b260" },
                { "gl", "4db41fee4b95f90da1b1baa203cc3da2b164a102038069b0e0b5be3646909967290522f4ece79159eee1b75acd01eda3dc8cedb1301e9f5531398770406b4513" },
                { "gn", "dc7b147c7e5703889b26a5ab9d7f016574dd0fd79fbd053a93f9fcb5d72e800c0c8081009e0cf18a04218e597486afc64c21325cfbf693f061be69fb223ab6f5" },
                { "gu-IN", "ab9414534153b74ca3233d4f58ed69be7c52c54edb148261f3c50810bb4e49f054ce970d47b23d851d4d83426d0604b86ed1aed0fb9f1713d977361f125da586" },
                { "he", "aea2e62272fe63c6b6757d3f1d86e4b872ad30c0d06c38b55cc457e33eb78031bde707a616d5cc10d5170823cf688fe4d4dc429c68fbdb88b25277efd1c0679a" },
                { "hi-IN", "38143bf8f816feec8dd49184b586654bf03a66009056b87826c24173582dd93553f75bf41e62adb98a2af729ec620c2f0377287d49104fa813cd2ba871b5cf31" },
                { "hr", "6b8c25193fd695107dd55a54ac6acfcf5c9d96cfcca7741e6eb11b4c85b9f3e09f8fa433112f567b0bf5d9bb24573b82906258047ff4c7791995dd8f562715b3" },
                { "hsb", "5d807890c9cdc9567317e6bd58df4bddd2225569a2abdee736ff5c61d9fa63ebc7d49ac12dbc08c5ee937aaa0adff8162ae3e5aa970b4ca18d0c00638fc93dd6" },
                { "hu", "c362694245abfd30a91cc03c49477a56e19107ee460b2c1f1f023ea0872440b8ae6d58e89177e251ddccd8129af47e73e873e4515dd561e3e156fb7049e3b6a7" },
                { "hy-AM", "0e9c313eef79101e9abea08095a504035496e3490771cb159c3f0daccc50835421e1cbc0246658260e9b70f044bc511ce28680c76b777b560258f2ebce792c41" },
                { "ia", "4be19cc4abad2b6c410c2c94779be25fb49efc4e3c72063b3f048db8b55770606323adcc78c2329ac53d3e79b38c8c866ddb9ca6ee380e4da5de3082ef4ab194" },
                { "id", "689fc9e0ccf68cabf65198cfdffb63a5be1a42732210377de9ae8ecd21d1625e921e629b75f5fae3a12f9dd8c03cc96d4542bd05805fc629a931c6172cdb7b0b" },
                { "is", "a212e36b893190eb4f89a5fe46b1633463af1ff2b83266b3855f057988cc8398158985ea5f557887da71aa96bf4a068ec30d3a2a946e9d50d13063183d7868c6" },
                { "it", "6e1999a6b24b50db29fcae645530eb1cf815aaca11859ac75b7aba545c4e732c9ab45db010bcea21945cd7a483364fa487c6e168109b53c304f78dfd1e0cbda9" },
                { "ja", "6601c0673fc1cac99cc7cc7105b55214a818f69c1c251dee0a86771c84af14d4c489d0566c2d53b246b3fb78419ed3a034e193e7138c3078937bff24712a7467" },
                { "ka", "78e2d574118134447ab913a5889c1eecef7b568a5107eebc9dc6d72fef2c37e02bfd06a4ab5b0ff5784cf9718cb76c8c08d9a80fbc0a20ceb84b7f96d054fc82" },
                { "kab", "e76e5f35d53ca057be6dd20d9e0bbe73ae80e1f38e611d6ad800693640999663b7b8e38cb50e03125be251721247e592fdcba82a32b95c7e6f31d9224456733a" },
                { "kk", "634c32ee67b7c2612c2b3878948ee4197ac18e1bbf924f300f96069671518f4efa90932fe6c61dae795cdde7681b1de994f5136ac5f32ad33a7dade77c162d05" },
                { "km", "7c861ddd4d6d9b72c21934f2fc5e876671d7ceec67b8b84b91831e19ee5fd216935af72b715589d004e43366b39f67bd2c24e1b21b065fa09faeae0edacf6c01" },
                { "kn", "6411e8dc1c807cecb79d7aff997596f3c0e9223d26c1269bda6505757a28f0ebf873308dd3d19ba1941b696d8897f376f5a4c2975622379f007fa8ea24e2e287" },
                { "ko", "491b9116b6cc4c5f82611ac27ea1ddba736faaa633a695f818f245864dc5f590bdfc7a30f4cbe558d42493e53572efddcdfc83c559478e49dcea77f05a90cdca" },
                { "lij", "35ec0b110184e61687e415087d52b564ec4c77556bbcfb42f385d317f91ed2c570a8fd1fa66d1bf4a1f35775876a88e1daa8985bc1227f94a15612c5d533205d" },
                { "lt", "35c395fe7872d7ae8a68dd6db30d92523e5a832b6862946697402a8237e558769418bc4903d4e209009d8ecdef3fa4f4f7c754a394d1f5d94307a9282c3f50cf" },
                { "lv", "af485ce153bac64314fb56c237590798c6234df96fe6d933de8d5b6e1769fe0f7efe1b9efc93b6967cb150ecfaf3c56a6965cb8b2a0332ca81532d16c292d998" },
                { "mk", "db97809663e6a2553ea13a6572c47c7e980e4bbc637d557ab2e2d40a5b77945b69a9c3761753a816c81fdcc156bc4c1d8c9466016ee67ffb4f65b96f515ca78c" },
                { "mr", "aa11b53edf4481209c8d9825b0e299eace3e98ce2ffeee092572cb878086c940b0442e822f5fffbf5149fd41ba08dc7e8f0c91a205641c5859961845a2fb5c0f" },
                { "ms", "9e5e1c5462d276453429148ac5a7addd9e56f62df351b3641f810574a2f40ed8fecff16bcb94a39a742392015a97142a9adb6231df7272fad92efeb86a453ca9" },
                { "my", "1ef17ffc907cb74def75542ece660dcda1fa3001c39c2e002867fa8e23af0a8bd5694a43fd82f51da681d38f4ac24a8a14da867643c340ee81f18d314aa1c704" },
                { "nb-NO", "65d4027c9a8fbb8311f017012bfad855e2db59adb2df933c8ae44e10b806637908d0d97c8f1741c5e4d04318793adcc7f4c8d38c498da72492714d050592eb9a" },
                { "ne-NP", "77e80641558e0f4f713067b7479cc9fcd1c0d6e0d4b424b512fad8a1f28ad058627d677c0c964d4481b5438c27cbed045c7be12f490e3477845f75bdc8ef79fc" },
                { "nl", "c867186ed2cd30accef4cc69ab71849cae4df39727b429978f14fc0f2fa0b941ce548ca92b34f039ffce770ceb63facbf6c4b6d87e26fd3b9934782c3e3d54cc" },
                { "nn-NO", "91b20cd880ba7ee3992eb895bab4f98e3887d16aa087e2fc14447b89b7998531130d35b1a61fada7a852e3ed43270595c604e6e03de83e3d2cda337d499ae0b2" },
                { "oc", "f57a6e0da3bcc181ff00185fe74c251cec056fbedf723f0cf28f1601f1baaa058a0738f274e300b704f68a59e6703428fb49bf69a6dcc395a5184d56d1a2c48d" },
                { "pa-IN", "519588c6152ba03be0225a398b6172aa3a3f7f6aff5e572e4240d986233a2014cca1a4644cd05e58caa38dc00af6cf8aac9b2654ac5ad56976916eb1e83796c5" },
                { "pl", "b443b076fbfe744ef741ba1167f97a9a6c275cf2d07c835d6d5d4ceff78558e8a05ab577db2cd36a03fa983d8c82dba30e69c6ea522745589512b28c04239ef0" },
                { "pt-BR", "5e82a977b2e26e320e56562cd445384ffff2381bb09c0992a278784ec1d3fca657dbd55bbcff771cdab3f1fc3f4df4250975c9858f6f603db8ec1b3be244dcdd" },
                { "pt-PT", "0a7b16ee965a578410feecfdce0710250c2574a2a29b06210d5d739b1fb4a65d5c98cf86f691bd7f7cf098f143c991978c07f71e3a5482667ee144b9f4ec26ff" },
                { "rm", "46e478fe5cc31c3b7c91d0f22389af7d44f828bb3c1f476ec440b3d95759ba674e7db0d42d126df996cd0b08455d2aef6d5e2a5cdcbb2814e6df430d50440089" },
                { "ro", "890b22d43dd32df0e1e392544cdaa8c506c7c4c1f2f51b8dd03423414b5361e725902431f749f09e2564153f4efcc215aba2ccf942d1b1caf69bed6b96933046" },
                { "ru", "ef0a67ec0b4b55f23a83366566f64f7048750ad3fd4acd8655fbae2130f52bd832e5c46a83a900eaee3ca7b0e68b64814b1f54f4422e099bfd2c1caf881dd99b" },
                { "sc", "fd03b32075264cece9664d7673149fe370aa442addef97107b7aee1511fc2ab882e542aa21d8bb9c889a5f68ee5c89c60a615bbf220f8808ac14023859ccee61" },
                { "sco", "780e4f49114a2acaf6724d74c81621ffa366aa3acd91d8c214d595699efdbdf87198279081fbc270a7d4cf66afea83ee6e4a69ef6e17478ab053ee40edc7c724" },
                { "si", "a8a815253b29484fc93af27f176dbd3650d662e9605d666196593f84666dbd174f9760d9d470130df935b7ed80940e45744c77e02956eee3d72615b86788d421" },
                { "sk", "8214a46f9ec3407a72d3d29e80c835594319533cb2d2b093eab48fcf9b813e691a1726d1e605ea15a8113de838be20eb6a5dda4087b3357efc4939ba776f94fe" },
                { "sl", "45a7428855354986215f748d27334bdcf6c6c0d6d3563c8ea47c22a3dc6b659bf1570ef9df318def650f60a62c099c3e6a4ef4f903222fc9a07673fac2946d3d" },
                { "son", "7fa681d008e0d59dc6640285afbb13675fb68f6b09b38ddec17071c29a290bcf60cfb46ff5f46be764860a2c5311b4b97b600f03ee40af78dcb41f65b11ebcdd" },
                { "sq", "c8d9a3b819233681c3c8ecdb2f1c0d2ad0199c881e06c12a95dc5ae4ecfc09f4c762e8996a5bf91234d6b274cffb7d91bfc8bff042ac9d1469478652134b541b" },
                { "sr", "41fc4245e989963e6cdabf4dd57140a0e8a30c3996e3a6c1dd654aecfdaa00160f1cf4632552e46aa6207b3804bf04d5ff34bdc85ad78179251f6d5ea300c769" },
                { "sv-SE", "5a742a837a691cbb83388d4aedac0da70b99785e3b44b9a33b10e5fdeaf549ed4e881eb5d8232bda1f754ef881d9cd931a61773b406536ade2f908e51b4b0fc8" },
                { "szl", "e0223a8429389f66c95e1f0e0d88d5004f38518e84e60abece930680457343593c87b3cb2f6bdd11daea08abef3f6ddf666f0e22d1eea61a00dd5fc6baea0d9a" },
                { "ta", "860584b1b9dd686873ec6c7d1b21c19d0da975881e14c43011a5a4548a9395f6f5e212fc2529010736aa8ee9270fc196468f2cc2ef52648056702dd555f22d30" },
                { "te", "cdc280fc6fb7ee4a3a64bb4cede3d6a3ac2554feb7ea2ead1d9e6854a3fe6a83d825fc5da5c7a6931fb20acc6113095285520985d98cbe3e3b51f229382cc1df" },
                { "th", "0fe22cfbde23324f14fa4b05711eb1be6d287f138e8bf3de5bbacfc86e0f776aab2e65ea90fbc83d0aaaa70a3d6a68955235ce1d4fa81e57bb6555e11571e071" },
                { "tl", "6592d11043b9feb7964858e6599c7d2139959a77f47705a999d6dab9d52b6a7cc3dacc975f9152dc4561218d0a7a13436a5355b2b38422c49d47078147a4666b" },
                { "tr", "70ea547e8e01e00a1003cd3b135dcb35439d9d7c6d7decf272c822bbe51184fdf372390ca61642462478b03e0a72b674b0c5939af882b9199a6a5e9af1ee55b9" },
                { "trs", "bcd4eb5a54cb2647cda9ebf16c22c4dbb5c471c48c32e6f8dd242539b4a1ca26f1bd54ebafd9e7884832d8c25c6666013061914393e84d7c4fb7aa79b6ad3773" },
                { "uk", "36f67a03386fc39e33a95ed4975822dff10e5703c160a9da865b0a7bb4f0f017a10de3f34d50e56fa60af77fe81caa8025d5b58e1a86c3e7f5c1fb2b5b3803c1" },
                { "ur", "2b79f42194709690ffb7bbf28ddc8a377b1ae120f3e13773928f78b872d6e6f4dfe49ed7e6960c5d240ef588e6af4227a67ed4221da3034c2a05b4059cf8b66b" },
                { "uz", "7d870d2ed17705ab8a1ceb8f11a81000061bbc14fd877095d3976dbd3660aaffd8a828dd7838d9ff90294a2979833109f7b73918aee6bf84bbf613358631c0cd" },
                { "vi", "15371a146c74bf7b6b56d40746bd8192291eeb92964360d276d99c10b14d4f5e25b410574c9f691d0dd204ca56d7dcc1cc9e760b841856a7f35e6b22bcf96425" },
                { "xh", "0b967f9f6a6c96f3cbdab5d9f98971d8c02688ccc6c3b2c217a3d7c62907ccf4b2b588e092d28fb42cc04bc4a692947668c2cb444560b13fdf285f75a77afa05" },
                { "zh-CN", "b3b65607ae4ac8fcf7f2babbf559591d345bb6413fef9d96d0eff336754ddf3ef3ee2c0bc9aa0c361ad690dfe1221bb39222363a6b965f15f4899b4ea7468a40" },
                { "zh-TW", "c0fb42be599c91fc572ebcfed3aa84e7e45c5e176d79be17b239d03545a2848e30af6246d5ecc60dbb21284ed307b882380123cb4eecb750fde4face0ba78706" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b3/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "e78059393af24cc8fab320aa44a4d35eb14edb12f08039c3480b8907da80d34a933bdc844d309c228a4e351a5f25dc41e4de0a6c0748775c1582d2f74a28f594" },
                { "af", "2cd4770bed8d490aabf3a4e245916bdc13c0daa6a2a68b78ecd2e016077e1be4ce284f653c4a0f1585c565f652a15693a3bf1fc79750830e6aeb1865460e0cb2" },
                { "an", "80020ce67f7ab608368875902ef3a34dd39a48cc43a6256da8a74ee5eba8e2ac5fee7c29fe11bb0d9285c3c071fa634595f47aa7a4156be5d8d0fa5939d99a38" },
                { "ar", "cba2fc175ebc49a5d29fddbdc890cc3f8d77fa6390caf53a2b09726d8e929ffb23275026948ae92e0ad225773ef7a7c19339ab1a9cff7b4abdeace56aede5273" },
                { "ast", "2dba0e2aeed85687be10e0e75927349a370b2b8c4c6a8d6cb4e625f53c8e8b4d51553eead8ff089159dd8dee0e512afe568be6293509b3a07f2d0dcb46dc9f58" },
                { "az", "8fcb18400d03a2add5355ac39d732b64a6ea2b61056ab4ebed95c5e3d7b503a8fb1487df122658383a768efa72b225a7c4c610d3d4f313eed96f472e08215b4e" },
                { "be", "8e486881af3926ef64f2fcb5526c2fdcf92dbb5307fbd8fc01ec08ae8a56874d5e0dfac5398e442eaae3c11ea65bf45790a632b6b5cd57daacbf984675d76022" },
                { "bg", "ff2036713b725c3bc918d7927b5ee141f8e8ceee785b93df3bd7e1455aa6ccbc2af39f984da70c59634c554e8b0d94892c2bec1e4503cd92da435cbc3887bc1d" },
                { "bn", "b7dbc9a6ee307dd045bff7ff5a4c204ab854aee1d98483f186f16aaa94d072a8db4947e6e6b515961514fe739f37cac71926598dd039288a29b2af7883a15261" },
                { "br", "05d7dadef4132079ce686f4bc51354e8986af19acc80ded46545c6cd2d5c4620adca6200e183a23e36ae2ee63a43fec7d89c120a91e354f062c9914a9cf706dc" },
                { "bs", "5066aaa58ae213760a2ecbe4c7bc924011e6dd71cd4b597ccc2edb1032d0bbb08df3ae9cda04ca4afbcc38e6eea7c5d2caa535d640649f95f790489efa15c5c4" },
                { "ca", "63fc603ece1493bfa2ee513d1c71327292597acb02a9daf8cbca6f60245c2d046c666445835e4bd9b8592573adb8e95ea33ee8fd42b663a8879aa248d5ef280f" },
                { "cak", "a4eb459e3ddf69ffc127f24ae3413709a0a28206889b9b9f0d19dc662582bb380c8471f6d44d11517688d62577178eb18877781634d2bd90e8153d7399f8de45" },
                { "cs", "b941f6d8bb3691255987a05b2c327454138d2f4e8a8959b14cadfc16789fd7611a40e086321a9e4713cc40b23591c5a019d725ef22782d1c3c4ec260a39658ca" },
                { "cy", "5218684d2b4802ebfec086144a4b2f0fdd4f4e0552ad3b724645f936b98cd0bdc9399e92624d45e4d10e95b0f577279c3b999945b09b06f64899ec624a43bce3" },
                { "da", "26ffe854961007e3e547c723ddf1a3b208fbb833e98b83cc7e1d61ad3c7e53a0cac891a4ef23394ffef691dc4b0564f083553f4dbe8d1a5ccfa2de7c7762140d" },
                { "de", "3e40c49f5dd714127455cd970cc3a3791f39da54ea3990337d0767a4608e0735a5e31bccc119d4bb4802f3d11aea89e0364f885882c69dca12a3f6aa116c57f0" },
                { "dsb", "339d7dd2c9eef19bb2a796f813ea71d5c2b7e820725dffaf365ad6e254efebff84170e8a92bac192a6d1de95c723d314fa7172dfe48cb4843af6b5eb2e708d9a" },
                { "el", "2f856f3848b0967f0a84bf5b972dcc131e0050497fc3273522159f421a4833193adfa4e82f38e69650944cf5ce69456e99cb8ae8ccfeb65fa1f5538488e29233" },
                { "en-CA", "2e4223629f6213dff05ba6926bf2417693a84bdae6b26ac44b4038a0fa84f3253436ce0f2b71be2338fc8d63c87f043002a1c8b1e0396e598abf11e292404f9a" },
                { "en-GB", "8faf4bfee91df8d0d326acd2bc7a69fec156ae7019af28acabbc79276d885e904e7c649a78c95ac3c233e22e08d93cc756c8922be5159c983a461e6da1264483" },
                { "en-US", "1f354312a49fa32e83a6d54c28b5a520c50e5164f482e95299c8f97041634d970d82860abb6b02afd1d4d8bb2e7f649f558150b361c8a830ddae794ecbebd7a2" },
                { "eo", "85b5d7354efb7bfa841ef37ae0d4690492a6f497988c0ac25474c0bbfb55e36f1f6bbb3306afee547a8be61dd23801fbef9c44a5fce439ef0a77e9d6d9c26966" },
                { "es-AR", "b871c04596f35f06a27495a79f77f49d1cd064c2dda375f8af5a5f250e6e6db1f794579fba46dd534c79a7718930983371429e390a18bf2fdbe2389308d8caaa" },
                { "es-CL", "bfaade0b11a466b631698eb10d733e7e208b0a712d25206a42fa0e22e8fb3826cb104bc0929f52bd0a701632763717a0ebd6cbf22f436e6fb129c1037aac76f9" },
                { "es-ES", "387ebac3092f516150f86caefc25aa967f2d36e640538cf9404db8b809da82bb30f147967ff993673318659a4a7c026b2f85120d4e40782426abad76620d3803" },
                { "es-MX", "45b075800162835413da76239d18a4a4df05d8c080e3629d06ada8b0e79306dce3f4ad83f269b8ce8031dafc026e171b0079ab5226dccac92f2574438d488e80" },
                { "et", "c229de0992cbed1aeff3cb2fbb093d3a30a3eea558ae512b5f5af7486db36a8c442b196a31f45667f144c6324d726642818c530f53d7cb1227472f441a72bac1" },
                { "eu", "7779aa334b8715f7d656bac088dc3aa52ed3617cd1b17801ac7859551b5c88ffa0d7a06da37310353d59ff71304a1bc117b117dd212b6c3d6151199ecab17fe8" },
                { "fa", "1398ce9f14e115da6ad189a168f31817696d1f4d641e260ab366c06cc60f12c77b17ebe7f379c039dc127ccc277c2942cecab1e2f5e36e9b670eb5ab88b494de" },
                { "ff", "5aad4b4bd3f106af1506fbf719a6986d3252d7f2f8146a34fae9bc6c0208d816c268faeb02b42d8bb65944d2ac603acb10eeec5f722328036bd10ef9a75fdd70" },
                { "fi", "d8d61e549effc4cea3be9f884c52ca007b91dfe27aa47f3bdc40aa0978a636d8c317ca010f13fb2dd19d4f43897aea59c3f56986ea6522426897bf66f3d22e07" },
                { "fr", "d7c196c151752ac3f49d7ceff2f019449fceaf95c5ead0f268f1f5148b354440e2ca2bf1253d2bce95f4918b6e96f385a54c3c2d1e1b6625c6384eaf965b0efb" },
                { "fur", "00be147d535e914ebfc8fa5b0cdf4cc0115e2151cfa7021b80a1a21779703d1b838c5b71cfbb69e154dcf8894b2359ba0d505fd2bcfc5b8ddabe8ac2f2244b68" },
                { "fy-NL", "b8a4139af115eb2c3eca159d5de56cca6d927125a642aa8812feca3cbcfa387cb838f2eb641322f16168b5733a2945f32ebb394165905a2d069a822ed4c2ad51" },
                { "ga-IE", "b4ea55c9e8e05a0e5d6cba1ef21f1dcfcad664d2b5d15dc012126be1b67f4777cd11a4a8e8d603e0b27ec1aeebed035e5b778952af353db7f65c469a941594e6" },
                { "gd", "6dc8369af31656a45988d63bcaa9bcbfbd8bbb1e469489f00a3fca21308f2ac97bd56e844d042b91b34a27effdc8675e3152662eb4fc5dbcb863c06da41d7dbf" },
                { "gl", "bb19b36723d0ac393a0dc4d848760abac454919802ce6aca16fb322bee5a0be71814f407e9f84d4cd6151482548ea7bd3eae11375650c437fd3ef556c659f947" },
                { "gn", "92453b7d44f7feb073f20f6c8afccbcd64db7a306a14b67f12bbbc3c6c650921944b01af7764d0f474006f8059dbb00dac42f65dd55b9d2007f14cb521a7a73d" },
                { "gu-IN", "2444752debd6681edbafe5ba8e049579e280e6211f6b182026e4d4745d8727cdd10dcb74ba270f39bf5375902358e2fe2ae38608c5c17b29731e6555158e0903" },
                { "he", "6dc7e61127a7d139130bca2b924a723f0e5afc78746c2b6a948e8cb3049ae8e45479cd2dd215adcd0afa52149a72e6d90459109f34e9f70417e7af0390a4fe97" },
                { "hi-IN", "c6a4997f25063d7a2af9883ec576835a5e8686fb35309ae0ade6abb94a79d246a4b85a533d3350a1228be22c1654065789bd2222350d936d9e436d832936e911" },
                { "hr", "4364c10aa24ef82dd5663dfbe8502cccfc63a565a96c08e4896cae0d71748071fd4cfdb852883ff1fdaf84911474e8af466ee3c6980d85141507f5ec5dcf4696" },
                { "hsb", "b9db85305cc5cc805bb3d378ffb8e7290e90a11bfca385ffde3535e5668593f298237e52d55ddfb3068735748ec16316e74795c97edb21b466f5f8c9725c052a" },
                { "hu", "b46b1abf69f8caa5e541b8b47807834a1bc4e25f4b09225645c97780a1dfaaeae551ee497633e5ecef577aec92e6fe1c1f276b125cb8ba8a430e01cdc920ae18" },
                { "hy-AM", "fe511e40aff84f556965ecbdd097d2419f659deca41a44a7fb715234ed8f4208e42805f4515388c009554ab93ae5b61fa5eedbc1ab410d4f253580fc052907a4" },
                { "ia", "f96a3fe42c2f721e81406c8df228f54887d90bde83277a2a800b1c8aef096608a96fa3105d43f15028f109c018a0fc9ad3eb2607b02b3a667185acaef68eb387" },
                { "id", "842aaa04242e7ce270f48422d199693c52179fe98064f1a9604ed21a4927dfd39d73ec834031556f0a407149ccf5fb6dd0ef9a6a7d31db5c27e3da976efecf01" },
                { "is", "aa032f7ce23b290fdd60b827c5377f68bf796840d47fefe52a8358ac9f4f702c902eaef458a985c4e5e7e2324b076aa1a12dc81c53c6686d3741ac3d4af8c1d6" },
                { "it", "613d61a2a1d75a9ec5aebe9365a5704eb384e461b5c3d1dc85f9f2faa814f174f9dda130f5738543eeb8ebda1837f5fe39954319251c3791cee9e1a7f88e89cc" },
                { "ja", "8f61dbd01a249779cf4db09fc5b669e664d28567cb94fed862f6c844ce22c8ca460b35b7b812cee236e6fd739e94e15960aa2b20232353d2bdff524b134cebf5" },
                { "ka", "a443063caf8e949857d467335b78bdc9084f97e108d00f1fec4847f30baca139b9c9b473a670645b9e52858f06040beb5930ba3540c612bcb89a380e3a6b3c3e" },
                { "kab", "bc263ba4b2ba71d49a74b541b3860042670341d1f0e49095846952f954108a8696442cb9f17131893e4d6b6e3a200f7270abb9a9b96d7f4b7296e426f142dcba" },
                { "kk", "320303276f016234c0c171eb876db367bffcf391ecb175e83b196a3255f0b4c5952bbb2a3470ab32a5e52146b5e01af9719161c7975fbf74b373adbbbd5bc407" },
                { "km", "db823e5f43dac36d7ca7119f8d2e56e5f6a11219ee2b21189eda55247954c3fd2c53c28b6b1da4f8bbfee00ff496cd3a39460b0226ed6e90e997538539c599c4" },
                { "kn", "e4d7d7e6032e14b728adaec08aa4a517e2d94bf2960e1e012c2fd10126e86af8b726ec2d20f12e9e159fd7c1defd218e6f16b412c79e349d6af08933a13a0a03" },
                { "ko", "03ade3fdd5f5801625b09c01d7442dbcb580386e317d37fab536c49743449c9d9acbfeec301e00e66cff1f2666c6e60e7e77a01b85123b28ff2f4401a7e7c504" },
                { "lij", "ded448c30228f086fefb6b5d6e3ba6cd00f56caf1d5d8a9dce20c3b6048ea30cc9b041301c018c8a17269f349a41eafa632f53ec3e872d534df628e303498670" },
                { "lt", "09ac0ba085fe34b7799b43bf152f288c5abb603c1a6da8899a757c90bded3a365695d46780f00cb95947cd23e4f6664a4008b173d020bcbc522a775f698d6e00" },
                { "lv", "8ba40e1d22bc692aa0ea676acdc3dd3c84b343d8cb19a680fb76df6f5e94db64c3aa3511ffaf130b18a5c3d2d599da85ca492e01ba10de065dd1b0af2aa5361f" },
                { "mk", "4cad209cfad2da6d6021452339a0c2a909297248df5865a1fc77951ba52914c5cf4490f0be868804ee1e371f3443d8ea164f92c34f88ebd3f0649ddcc810140f" },
                { "mr", "374c72f37cf1e6846752440ad4130f3df61901d58bc146127b1fd6e0599a3ef31f47fdfe5aaaa98857f98353928ad76d77bd0cf30ba1a922eb0146d1ef3eb65d" },
                { "ms", "1fbc928338c2815c549f4f1087885a7e446a5e510af86d15d71ab4caa88d9dbded5ab08536a16cf001e01867387114b826b5db55f6e0e64e20956a88f70fde92" },
                { "my", "b2c12336e833caacbf74ff01561255b26b6c7ace1c4346486517784c5356697367a78156c0703d95f52e4c01a66d5d7b8f00449a37ed555a5d9645f83e0bdd01" },
                { "nb-NO", "07c74b3b67d460f8cbfcfed680fd6eeaaf6e72a10b730ffee185cc4a4e88d87a97b14d6327fbe482d16c7a7711886c380e4e7d5076a6484a20158c9dc9eeabf4" },
                { "ne-NP", "ee42ecb2257bd44e15e3fd2b39cf5e7e9d31ba5ce850eb4ef2c4de0e30b71b7a51dbda0d42a9023032c93b5c071917cc18de32b2ba4b78c96c7aeb36430c6ab5" },
                { "nl", "f2ddbe95aa9492ac63bb5194c8d900d26294f4e07070a9578f9002857d83cfff34d4898b37e94b7df84fc48315994aef2250d7d32288651d30dcd17778f898da" },
                { "nn-NO", "12f06fc734abb06a126c6dcef44235fab1a5027e7aa3ff7baf10732c11ff9145704581d897e9579ed01adb623cc5994f7ae91aef804e3049433cd90ad8a77a73" },
                { "oc", "fa5bc9968ec01d8fa9d06847fbb4d817bf134a69d095b076570906fd469cf88b71e5ebe860fa6183807747d030224700c7c8f16db4036315676509a3a20e9313" },
                { "pa-IN", "5d894eea252467f3332290e52def0ddb275c560bd27e81d51f7fcc1d2936a12da12cb936f1be19c04fb26373db0db4e5f6deb65e8da9cfc4079888553c932187" },
                { "pl", "5f2e31d851b65c7825165112ebf7aa5bfceb994daed76d97103e204e8a319f3744458bc3652b7115a479989499d6359768e3c4f6ad9124bcf5478ab52a8f6936" },
                { "pt-BR", "905bb5ded03165498ed0f2e0a363967f1375447aeaa4bd13906fa3d5bf1e96a0ece35aa68f75f9483f1c9b9df78ea36450ecf9d3c70eb01f3c69ba4c281786de" },
                { "pt-PT", "dc6a545ed9e92105eee26c7338794bc438971086e610dae9a89967ea92adbde6b93b6c70f113fa25bfd71e4ff3633515833706f9c0e3c4d665f44827f9217705" },
                { "rm", "fb13d30a2b7fd17cb5fbb9443ed79abf96646b0310695382ade67b6f5420385164cdb2a78f714acfdb2f400bb66cbe19234d398702d41f57bd9cf397319aa15b" },
                { "ro", "666dd2e04b8b7d60a34ff3b3384319d5fc85b8e8b3165d19cc388bc8f2c184a0c5af2d9b9ac3e31f66272d6d8e05a88d9a75696200356eae59d22aefd0af20c9" },
                { "ru", "ec45aea989854ffe6736a69605176c98d252aaae4ace55217bfe1ba2b0d763d2a52e61cad76c7c83414f95d9431a5f984275d883d94bfa343b7c3dfc21cd5a4e" },
                { "sc", "543bdd3a7867ebc3368445812765d5e3d131aae8ebcd460d6ce1a144a1f3ba82f1aecbe25dbf6de82522ad230b016130f6ff5ef86eebebc5d08996427dbcaf28" },
                { "sco", "058314e70955022e8e3248dc4ceeef5c6d1ac09f7ad0909f18f6c611e537528eb7dbe434ae31fec96615d29bf634d77cf166fa55e9f158cc174db1894468a48d" },
                { "si", "606375e42bca8369cbffcef016d49101b6c17ad5e815064da1d920a6cf80049dd0dbc378105ae0b3d7697f5f525c99b3b05f17b420feec281b6857b5b0dda5fb" },
                { "sk", "d86b6bb5c113d1ed45bbefbbeffd7a070cf1fdc19f629f4a8b4cb3165b3a10c22982a37ac6fb541e541d4d8d86df13c604934ace5a9d1977a0ac3c0a8a106b3d" },
                { "sl", "ea7612ff4ef67cfde8a1aca24717e5d1c4842f28f377d64ad523b53e56602764859121ad651396ac5c73c9240db7d81c55ff0b24711066b832b46c8c59b65167" },
                { "son", "2842f366790708864565252aabd373bca331025e11a825ed5313e5df22462b00737eab4926b641ea5bd7779df4937f48ef1dea2942d54e3931f959719d8f638c" },
                { "sq", "a0405df6c5a56d62e2587398c5764b4ebb110b98b115a6f69070a006f1a59ecaba7a1c2c9510d0e8e67a849dd4c3ba22121aea69d31dd5c7d3b133a3aae6378c" },
                { "sr", "6ef3ca1d1244948cc3741fa75db21d0d861038b3efceed254f5169f773cd6c1d3ea656f587dc53108fffeee96097ab7990dfde243cd57083d4417be253043d39" },
                { "sv-SE", "ad3082689f7d102b4502226933355348d1dfb26acd3f6a5e963d1cc73ab39224f3de995e5771a82035bd4bed915c8b87334be42167d5ccbf92307dc092b9dcf4" },
                { "szl", "9030b08b10c018b2716a1b18ecaa558590d80b089cb79cb1b4202e7e62532ac83fe5bea7ed57191c549d7a2b8e5b8ac7d40b0c3fe0bb097204977efa1fe9cf40" },
                { "ta", "d6751c39cecf88a90cfa0217524663aa8e99e3ec1208dbdfa904d51d92f8e2504f469ad5181f1decca18d462651c5ed54336fcc8f67e7d7fc936e8bf64d1247f" },
                { "te", "324f8115c1a5b1d3247250e557e7f85cbefad700fdd6297285e01465a88078eaa937be4e2935807ee371655922864636657c7d239f00bd26082daded28f7d0b1" },
                { "th", "3733c2c634fe38232d919c08135789d3fb475bb86c921cb4674f4d7234554a766a7891522175025ddca621e23fb72471b5a6c2a5bcbb7f117e6b9d81796603e7" },
                { "tl", "a050d9432b332bfd92a2f01cb98f257766a7d4efabb58ef731eea94a4fda1208344737fefe73f9d99e327febd5a8b11d2b24d4499fa4a59c43a20f0b72ddbde5" },
                { "tr", "ff1b60ca40406f13794e1bb4ec8b804af963adf98b642b0206006de256ce32a1a9f3e4525342dc74e7d4584dc90d0053e817594e32bfc786abc16218a07b465f" },
                { "trs", "58d7d7b64f28b54536a7a5ebe09e0784bca7860003b7bc479f0d216f70afd52bf245f3374765ff781b6926a623f2dc94e8e20beed406f4fd1c3c8b68ac47cbf1" },
                { "uk", "86c63f574baacd3e30c8741d56392318171558e0e173caeedb46220e5a6850f2aef5ed4951743dea1189b09c2c49171ad7d7b251597f0c0baa12dda3bb1e5949" },
                { "ur", "561066f1a06e645f24eba0305df438946bd8e3b043185a97585770a711d5ab6e347ebebd8d534fa559df7c88fd33ad68c4e9058c55dbbe7fd14422dd42e7d23f" },
                { "uz", "6899ac6938791341dff7170a69887e2a0421eb9d22602b24ca530348b7148c61c2220900d646e73968999c9c3cc94c856e309447b194fe4215ca4e0dad5c7778" },
                { "vi", "dab20151415122390ee2632d52673a39d3125ed187104b0fb91df9c6f8f476df9371394641ba13fc84bd5052c5df010115a0b75505cb843aa248e3c4edbaf10b" },
                { "xh", "cd3473fa2dfc7a35f7f84f4ec0d2d84b36fd0c45498b7a4d946ad0edec1646c29d9eb8aeba46f64d8f34862084a043b1506daf86a4436c69f553365e71c961d5" },
                { "zh-CN", "275286c0005492906ee30e114ef2be069a838d5bc277c0d532de42e86abb4c5ab422bfeac44e9265107d11e4ef5c5cffdacfa13051485c1279ca599ade5bc927" },
                { "zh-TW", "56daf36b9cdc15613353619b41a73b74b0bd2759590fcee8d96dbb73ae4e021a2fa6275028505e9e58cf63d898f2310bb616de165663a50eaec62d1fa761b15d" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
