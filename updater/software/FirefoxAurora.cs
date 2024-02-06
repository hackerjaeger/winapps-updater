﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string currentVersion = "123.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "5b9b4e753409618065f392332f040a80785a96187e1a4c39d04024e84367276c2bd2bb62c9fd56ae89964bef918080b0ba249c9ba2f1812469858722a39e0809" },
                { "af", "409ca89c8840063dd8dd2c25808717be0976b32218ab5bc912b14d11743cddebc8d19a0559e0eda839ca89b606f435726ebf3c66c7cdda2e883a365b9da8bb32" },
                { "an", "e0357f95a613fec271c62a32caea7104bd583447be6c884700e28e9fb5ad950840b71e03eff6d7129099540bbde3da9d087a7e0f177cae6c15f472e2d3c45e06" },
                { "ar", "c4cd18922c8a2af81806a44f6bf5d55701775e1ee556e208b7933056aafc4e1e3bd5e0b40cbac774882665f06a9ccd28f77982a76bd40a25c9639efd0c00ce80" },
                { "ast", "d6e8a3c4fd841576c26888efaa5e07c461a9f92e832d8295b23265d469a55cbc071eef39ee422e07dd93acd90691432928397cc88e3fdeaf515b8d80a94d844a" },
                { "az", "b2242d991a171c4355a9c2ceb6083989009b41a5373e38ebcec9415f1947b0402b3c88956d966eb762c6002b6f7dab5733dc132b0bc7affb319525399cb8db4f" },
                { "be", "132c565d28006afb12ac055ab3e4099500e098eac9eec0fe8f4156d677f4d69bb0bc6df44c9abc559e9a8f1425a387702bc01d913979942d3f542e057d2c0d51" },
                { "bg", "b4d5dcd696d1b674adfff164ab56372869bd0c0acf7c430b330eea671a22f7060ed036f8f18b06cf21ddc9f725404c9ce8c81d0700db735cea4bb284df88001b" },
                { "bn", "b6842d4c796c3ad0b23f2c2619a70997104d66bd9b5ecf856e750bc6619ba44f3e018e86a511d7805148f9a3c08afa0bfa7e8505e6086ef117132c0832be9415" },
                { "br", "73ea4b18cbce4ab0ceefcd280b612402a949e2bfb81ff4d6cde11cca5f47c06f230d3646df28ebf32f369d5c196bf001c69aec5d6785005acc38a822b8810c78" },
                { "bs", "2c041c47e3c421c716b3b2eb2ad98964c54a8d871e62dbcf15e98783520d7b7faedfae4be50c472c3937dbed898a636e19f4ed0ef45a320985fb5e15ae5b429a" },
                { "ca", "dad666428a60c94179a6c8962f45df42aea48bd2ba3ed501c25e021c5b349907bcbcfffe5d7c17905048cfc47f469fad7e7c64f6c4887558e03ebe575c91c2b6" },
                { "cak", "c7c0710c6ada67244fa274f547638fa4b4a070156af214cf0c9f77725361da56a3bbf9571222e49ddd8cbc1c6e38d01c957d7c70f88f226d2fcfc583547ec539" },
                { "cs", "8c923e8d22c9e389b7e7c2b6d5543416d7511f603def191e17edba9f3fa996c8fa531858bdfcf0d9a17e87278c33c6b235ec27f22182ce5d2fb0d71ac021faa9" },
                { "cy", "bbdab02221c5830319c354abc259a566e9481a7df38584c5b76f8ffa8ee1400fa3542f650b1e3c72e0ac9dad458aa4d426494f80b440dda4c95d4ec1e29ba341" },
                { "da", "227f4ae45fb038a87b88cda10dc207a130d15d02e4b1f285596ccd08b5cf79f7ea42d1063ee43c34ef30cbb50260adbaa3bd1ebf56b92b67c3e56f290aa97c09" },
                { "de", "d7d1fcbe25ce5ab4d7861f0d33367b18758869a0bb974cbf7d457777c5098d5c4b8d9bc896e9f2d772b9f6ef34daef41ba8dbc9090b744f176a4983c0e0f1032" },
                { "dsb", "2cf5d85bece86d5eca7c82377785f4e793897a387c146925edc0c70516a9d80b79bcaaadc43dc18a40022db0bf93ede60c7a9ef8944d3dd9a562f14674e4e87f" },
                { "el", "6d219d9baac3561c93840ddeb26e0d186e50faf6896e7f57126f73a60cabba5d70af10d505bdf1679c97dc6afb2516869c8d6d91902c0c8359807b0d3dd90212" },
                { "en-CA", "b7d25564a752c10528ae3baeb6685c01686e3ee6a4239565dbe11e367e7bc1d1b90d48a1bd92cb732ca41f9da740f4a7759d4d065bf6b3734c66965c126ed442" },
                { "en-GB", "c06f2439d456ed3d58193f4a3e6d887919d8f4b0966ec9d8b3eb85a1b02a78da148dfb3e431e6ddfb086f45207768dd04d04093525561675017c04469dfa8c80" },
                { "en-US", "ebd6a42691e9a694dff705a6d0e7b125740beafcc02ccad20d63f8d47fc31de8f6b1950da806d0df7cd39f3c1cbfde4ee8179913e988aa0d78e2532f9bc64e5e" },
                { "eo", "93df6607fdf42af67cc5c90b0df307d2ce26a035c74c74606013c10852a3a58da246cbea09a50b1746b0a0982984a1fbbe7ea1a8fd4742b708162d4cce6bbc14" },
                { "es-AR", "7c892875c6b9c49024bae4a803b74f78d9f0b850512b6008d749fb80dc7a1292b8132c71ed8439c3e79087fba25a6a420776aaaa44732323ca13f11523cbfb48" },
                { "es-CL", "ee86759de6df16126aef4da2f0a9a06e2542d747f9c82750db7155a630b45e4ea197379be9fa7715372b0b7a873f6654e437f7f9d200d1049c127a17bb818ec4" },
                { "es-ES", "c4d45b5bd469461147fafa3f55f6455544744031f85d60da6a4b13ca4c1405b9e2fcf533a2b8468658db826bb4acb8cd311f4439a96e0b65e82e3b5b30c8dfbd" },
                { "es-MX", "11677ca1b8fbcd999e5f810b8f03f49a8f1b90933e33cedf7430b4037ceb18fefa249e25c8a8ae1c8f461a3baab6c4baca5641e052b7ddabca89ce71e5f22e4c" },
                { "et", "d383494d678a72bc646a01cbdd55fa0afe2622bda647ec7104afdda225cc9dc11c715a305ab8a6e341502f9d4b7dbf5710f48ebafe6645965553a6788e6f460c" },
                { "eu", "8bba833f4772bbf66b892b99921f03841dd50faa063e4391922285f6ef8095e46e15ab8651ce56164a043c6858f6b4009c34862c914906cbd50b5d9479f1efb2" },
                { "fa", "ffe1b16e5a6d753c3aa0b628f91acebee901e4078477c9eb07f8bb216a848f21ff41589cf003528e629b364fd671d41d697a8c43e368dcb5b771696be0fa0444" },
                { "ff", "538c905757515235b62ef1858db634813c6b3746e1955dfce71d19c9a80e613764e00e4dfe5fddf252d8e71b33384655ca66dceb97be8b2799ccfed02f09559e" },
                { "fi", "8968d38b052cd848fe7456a02e108430ee8e3087828010d88fb7f9000dd9146983c2606478af66ef54faf3295201fd478fe2306e9417a9f29885d11f62c187e6" },
                { "fr", "6bd4e0f10d824ac94ddead72f9bdb7a285a7a72e9b04b1a2f6053945dc2879e6115cd03224557e6374b4d882aa235ccd36d7e32d417ae4ea5f76fac4033a8419" },
                { "fur", "d642328c2da4df486e26e423cac2ef26b1d78e5dbf9ded1a5c93379a9022a6a362871200a95f6938bb858dbe3f75d88a9171979e199c822b1227aee368668ee4" },
                { "fy-NL", "35bf9718506de1b58ceaf6f376603f7818aabc422908af78f726183ec734ca884e6ceaa326beebd4b52b98b9f91ae2d7fcd92fa6a63f7dbfef18b44806644e72" },
                { "ga-IE", "6d2e6dd2c86b06d45476661e3c8e499ad33a1e8768a184b411aaa4badf607eaf917d305752de5f7c2ca93afbf5bb07b8e2da4e3ae2f639a82fd7c4b0c7dee036" },
                { "gd", "7f0094f8c7eeeff33f8a0c040a1196ff0f91013270ccb833d337d6cd7cd82398245e0cc214f14a15dcc8fa1ff9ad6628f57c17fc376949b24bcea0e83576bfff" },
                { "gl", "0cab2d689b1808cc1cb34d64b2adb4a37cb1d3c1337eab0d0b737e1bdc4e0face946ecb8446d518d820d27a1744753fdad09e8d4ff0f724d6c2d2345cf3c07bd" },
                { "gn", "55aa83d107fe0c448187b145c5479d371b0b66c5dcf00a07948efbb78fb017bf683bb647dca6aa826753b58e5d4f1a3173376251b264ebcbc8b04cdcd93c3703" },
                { "gu-IN", "e56807757f74e64e2ec754a287a6966edf087e6b19a333163fa2112028116d9c21bbe19a566605d332f9e8a2597cfc79248d8048f866b3592f6a794848a58e73" },
                { "he", "17496a4dd2ddde0f54389d2d45e892e2afb5944ded7d604b7edc308173618eae89e44351e06cd46c62f991ac534265562e4ae56aed48c5d275748de458105182" },
                { "hi-IN", "aa720c43c4b03fab34729ff06f64516882e0665a540515b09c40ea7e2cad9b7ffd789b18e498a0f6f10e6a960a790ce90c245d9e25026008b3d2e001f2b8fc9d" },
                { "hr", "69c383aa8df87442a49cd633f824e544e7576214639ea7b1f8296a90d2e176dff413283e3dd8a612ea054aa25571023b3ee183e1aa689773bebbb8bd1566f635" },
                { "hsb", "1ca9ee060bed11f1a37a18187cd84e404c2d751342166b2dd883ccccfbde842945e8e7d0962550f436d46f477ba1fec02f4f3fde2132fe48c44d2d1614786fa1" },
                { "hu", "49877ebc26f4cc64b8147c75f4d3c45aed541366455a74d7a54c396820b03cace4d794507ef865904c2e1734a086c236a064cf13bb60f7284d4ee77fb8c95242" },
                { "hy-AM", "414e862eaec525a1a743d1398ca4c5c775382d2760ec7071923821b91f14ed25a80806220167b63f8e73e044dbb4979e6a724deed88c432b3657d1476fc22768" },
                { "ia", "1acae4511853e85a3d368601a140ac17e64fab8ee4215497d022b655f18e1d27f62f57aac74ed96f26843d5657921b47be9fd93da96786f4dafabf8f22dd6615" },
                { "id", "62156f762d99584b4b82ae5898cead804445dd9e0b25510bc90b5aebd174bbf8f4905e0fe61550f5f645602cb3aaa46fccde8fd0ef75c3dff16fbce11808e1c2" },
                { "is", "c8e6e0a85d863d8612522d4ee2a95d3ef0c5791cf1575ee0bcdc7be2abaf35d1e22fe349c5478f52776517a38afccb46bf2f2f46935dbf3a71f8d438a5323d4f" },
                { "it", "3960f95f8b02150ec2319edde6ed214abd81db49dc0c14cfd20be9aa1ccec493d02946333ce205b8cd313d5bf86c2713b6c258ac98359e7b3da9343ea9996e09" },
                { "ja", "d484566becab32debd0c5e01c81248c456cf564abe3145ffde23b869b7599a21e2172d8a6288dd057a7409d959d30fee48d01fbbe9b3b39f0db53b586eb18c1a" },
                { "ka", "f64d672fc23beb9ed8f46dcc3cfc8153340238789b72f9dc0853504d063f0ca1e68ecbc7db325bb64b4eb28883658f4063da326c6e3b4b13cd4a171d1a125cf0" },
                { "kab", "6f031ae8ba13ea90dba21702f62a009e5c2592ea4ebcd0a20c24acac2ad6e3804549bda14e72ec234cff165d73cc9c7c7d3dedcdd4478b104a5b4fd4f27b893c" },
                { "kk", "42dbc9497245285f19c62a1c2351ff0faf84c5a45bea840d14a773aec664966885d87478ddd3b0c798939514c9fb8619bf7c5f3437d7233d4052e01fcd9a81ba" },
                { "km", "7840b6c8f6a7a7440039be2fcb6a05d11e516283c500e46dda2033efedafab23456cc213ae728301291585095b5d325afaf59465ad416238a638a125d1cf743c" },
                { "kn", "ccae8a8f9817649b029b0bcf0e6f6a9fcaddcb7e3596cc96b5e87967b53ab5357b20be4d92d772b8c9587119392c623671c36aeb3d7b5e6b34572b66072d9ca4" },
                { "ko", "b6cf515bce2226b10d56bd9d68e3f904139d4d50856a930b8c7e6321078b1a86af2db5a55ceb08f1158dd8f0007bf441d77421aa96ae1c2f8d0e9a39dae615e9" },
                { "lij", "99186e5f2e7d13b79d50c2ea7fdffea286ef66d22d2fe75ce56c58e2a4c127e3f3452068085fe02898cb8294fd7b6056dc3c5f202128721679f30f467a1e81e8" },
                { "lt", "7b7303a957dc058f76eb0f57c8bd46ac4c96b780cc74140a742ea0d6a3871caf16e9060272e6833a01840c08150bd55581eb2d8004c4c325395847e3ea1181a1" },
                { "lv", "4b4e9a406a1f26f86328980e74af82e694225b082830e137161cc7cbc1411c1dc1cc160dcc15e519cb897a6cc1c764dda4efbc095feff5b6f4d87d45ef5cbc7c" },
                { "mk", "302675b9d33e649fc75b84e2734a792dbd18571518ac614046f38aa813aea04b3063150842d5b065dc83f36b71fe21b7c010e2ec1531de3e626013c1863f6555" },
                { "mr", "c890af6c60807c76304aea22a6d2ef29ac07853f66df7661257e473fd80a9d9e6f1dda5a46db713548551ceaa3f28c26bc8ac2182be9e51ed645a02d10c9b433" },
                { "ms", "98a66ad81a73588f7146879c19655d61e27fd85cfff82d1ec3b4c860c20e0f7695d00c6955d1101a237fcd21cb1242b2d4012a1db0f780cd0e4e3c4dc5c1e744" },
                { "my", "0a69a68fa2b920d9ead126fa140eb8db1babdbeccf5183873a8d94cfc9e7ff3117fe89f803d54df9d09f21e5f86dc2fa6e312d96ce108732e40901f5d7db0813" },
                { "nb-NO", "dfd8b3834316649dc40bc4d3d45b21cac323ab24edb4109ae25d8a4ca5fbb4b249a79f1b0e57797c69bd92ffbbafb049aef580c50ec2674bb59f296a21c89bbe" },
                { "ne-NP", "c9d776613965a8aa222dcf353e869d49498d94dac1a2cdf475a342bf7dc56c7ae6eb43e6fc8e529d36fb1dd5c87775712c3ee80eedae6839fa24a26e97b1d309" },
                { "nl", "9be0f9f8c0b8ba601cd8c8d023bbcbed5c4634076a71359d3c037bfaf80ee65634d52a16b9c3185a9214b0ec1913bc7fd1bbb0c2a625f234e3ac6b94c33bc384" },
                { "nn-NO", "b1c785bcc5cdd0440011e762f5557de5846c3a4ae774c24d79eda97265f0ca31ac59e6f45f335a0a58bc19efb053299872613109040f70dcc702aef9a5f9c8e5" },
                { "oc", "b1ac7278fa2f8e47fa0b0a21266e99e3af27f1c97ef33730318e64d36b68343a3973e57285159989f94f1059c179c4486c23e581567b6bff6db8f7d9c814b4c1" },
                { "pa-IN", "e04ef4fa8431f5077700992bc7b0a42dc5abad35e64d23b9b4691c6d15816dc7cc22c96e67feb0a7723cd78fa39d77df20890978fc4b26a462e8d59f6b6636d5" },
                { "pl", "7645da27d8b638af035c114668e3d79bdfc35558b97e487c4abb5776adbdb9f3839cd9996a2dfb99e077498bcc58972d5ef48cbb892e5bb2c7d1c93c84fdd74a" },
                { "pt-BR", "e2a3e851d7437c82839663938fed2563644dbdbbf65bbc57acd46c32579360ca0ca2d775045ac7e235f308e8e74ac388fd266ab44c531141eb14770bd7987cab" },
                { "pt-PT", "260c055873b7e4f0d2a8db8b7f394cf8bcef10dae3e7f276856eecb4e51ebcba3f19433c0f82a2b9d04dcb96daa3305f2955267e57aaa5c8cb14f46e61cf901b" },
                { "rm", "68123580f96766e9c4a58ac2dd13687197cb0571e5dd6963c7ca634e52246d8b419654e499cb23d51eda98a9c82a3a97c57e98fd7885cd6fbb4348cecad827d2" },
                { "ro", "40ce8aa4519eeb74407054972b1500ca06e3c0e1d955b3f3c73980344277fc43c91aa7606ff24b818311da1097ae9457bb3b2271bb07692a3b722ad22cc6f5a7" },
                { "ru", "cf2302dec9e7c68cac4f8b78db9027a3762fad41b3478ca2b517eb7ecf53c657be8d4fe3728bd1be9141b30b9d4be39d6a8236b6b20facc601bcc0a203c2651d" },
                { "sat", "21b2c099ab390d0af12fb7439a3d61ea42577e8fc71f910c3941b503fffc8d99861e587cfbb5a75b88984d60a22a6a09ecc857640ab4b34a6618d21117cc125a" },
                { "sc", "b5dd681ee76f8d6a6ca06c80632e63f367b8d5a566d500caa99b73c416c3f6b8b8fc7f3381cca30958115684afd756817ca1c08cca1ef6e5b19c5ea72b99ece3" },
                { "sco", "bf0d7e8f2e2860a8e2e8e0a830f978bc328e45587d665769dcf0328115c6675f28904d35f625bc2620a64d41d570b9667891e38376b89920361be1aced61554a" },
                { "si", "69b398aaadc7bb11cd47b4789fd24327f2533597594216bf69c1b8531c20f8e23b4a289df9893975950a6119b60f56b262a985a898cef16c818ceacda446b320" },
                { "sk", "53a41540390627dfab8e61df087fa43fc2e4309db0ae8f30cece484dfb2adf8c66174fff67c94c2bbbd808f41cb5e3b5450895f8be7e5e480f19e07acac7e19b" },
                { "sl", "9ea69614affacfb62192dd761134a2cf3640d79841c0d55e3680b03f510572f58dfa850ad57d6685708da31ad434ebdd2a623dd3a55ba9656c2c02f5247769db" },
                { "son", "189904a53e05f8e2e9384ed34a296eade75048d101bd2e8229d649996a4c76a13a5cb209cdd835e93113b24f3d0d0f9e802c03b56db940ddea3bb069cdea2cb3" },
                { "sq", "000df2405002c69e5181cad6d5ed10bde77d2b8c02cc91a519dcf51b914ea27fff6b5a14c22aa9dc007007b48ff56b81ab7affa3f80a54cda3ed4367ef6eea44" },
                { "sr", "738feb4258dcf645ea5454251960aa912dfbaff12613413e9a0c5b97f0d488391938ba727ee60f6b6a3448502340ef1917b16326c5a346939e35c2fca3f2eace" },
                { "sv-SE", "70a539c405fb950fc912705b8add0290e72a6c4f1cd31f7343c2a5844f4b963261b0000b0c1dd0e86e5dfe657d54a9bbad487ee659762b268db576fc03c680d4" },
                { "szl", "cbfbd24fe56b86a092676b2bdc7810590914dc0129e90870134518bae916517068eb026dddd518f9bfea0b94817fc79d091c7fd38dadadfb5ddf82b2a3dfcc5c" },
                { "ta", "42e43d043d328cc6c60122c968a1e66b1e83bf8f3b7ee421fc4978e98da2b31d608d2a1ea4d24bb1cd16eb6acd6e3ad09ffc8103b786fe26beaa0777b0dd401d" },
                { "te", "d442ed0e4afdbda751b343b44592d3dc96b85934b7d64a665059c75175b81a8697858e7e6e5f93d4c2300f98f69316ee1d31b681d62bf8410f814b7bce7f978b" },
                { "tg", "006439d02376d723f022136c0e821b81e0d2d270702d933313a089331ae6768013a4d6d6712bbdae9e3fbfab2ba6afa7275f805b99c79214b727d577559f312c" },
                { "th", "f078645bb1a2d5b441f9399a777d3577bc0919240a760bfc3276b59208b8a9d6c33771fe18072aaff9481b0881fb96864d3d4641a3f8ae092ac2dd4f8171702d" },
                { "tl", "760ebe71fda781d54dadcacdb58b4bc54c27751e7b222aa794d5fcab6c4f97bc40930f50e1286f04ca192483521c852d877575e686c99bf84cdaa06cfca4d91c" },
                { "tr", "3c9e928488f416642a3606f1d4e9d5fc8a9cc9db6a393b68c041bd5a9ae978881e22c0117dd105355b48f112cdf3f3bb8b9220b8957b8d511de9284b7d315cb3" },
                { "trs", "09022f614cfd196845a3f9713a4f0d7296d077efd9720555b05e579ef1405116f84986c917217c4266691d9429514ba509ccf2d7047cd88a4a5b0a3969020fa2" },
                { "uk", "b782d1a61f2c555d7f539f316d83fa76a33fbf09f0cc1d93039bc39bcd1e623e884bf803dffaf21246f4a4fcee6cfb44ba3b9579fad836a2925d948607dca91a" },
                { "ur", "98ca8b0d61c405d8910fa57016ae2261264f55ff4bec062c953ff7fc1714e593a838829e4b5ce85aa68caa0442a2e3c122d18b802a2f797b49a52873b3f3032f" },
                { "uz", "9cc8171f6204c4e2b614eb3e3e70f4ce89d506a118753073c31af172f63457545d1894b8cfd78ec4a10a8059755d6204f651c49e1c1f872f3a96f1dd949ff323" },
                { "vi", "e5dcd0d2f8cc644ea21355cffc0c13b20de9f224eacdd1863658c77c43b6aa3d35ee794d54f2cf2dbe049de438f84c56519bf3a84c69b62f5f6d03a429eaf54e" },
                { "xh", "f9c43273b098728dbfb32659256a275a6c21b25a444b51cb904245cdf1396b48f59bd441107c8fae52f369c189647bfd73117c77e1063b7e6af0a0d6a28e7a30" },
                { "zh-CN", "82ad64ddcd206f590d0d2319b6fcd5967eaef3da8c71352972d48f678df9fdd24132a4174391109134c9caaa3627f04d2947999cbfd5f705b8e16a23df31fb1a" },
                { "zh-TW", "c9a76c7e161bd11b3b8ac640bd5b7e2c75d12169be34f48bdc1dddb9c6d30b454860a1ad961fff40596863bb2226604c169a7464cb996694fefb1573079a3f47" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "3ec221c17cb65084749e56e164493d9461696e6870f9a2901b27e01b00641c169f22fb8d6642dd0edf510f2baa2e3df0ce151096cb3992d869d12d92d7e8a004" },
                { "af", "f0d33ffca91f2fccc699786d9f6c6894e9ecd5cd799d5af62749ffd3eaa94b3f6e7413956ddbabdd4507580b9b31cd33eff95cf016528e335181e6d8a5f05ef5" },
                { "an", "28279c413db8f07170acfab324a481549ab3918413391399bda979b083f01d54594722f270271d04f507b077864955a133983edf22b87ff9a20dbe27a5e33295" },
                { "ar", "480ecdb44b8a8f0366356b5acad07c4e5dbeba6d5f55646bfa3ac052d339453eb047c47cca54277b3daa2030fd4dab13375dfbdd2b4167277421f8a1d65fc292" },
                { "ast", "f541a99a053c7d66ae3403250582fe07a0f91dd84bdf7ae0ab4fd958313219e3feb9764844c604cb8ec77507a0e5bd7e1ffb42fee2924119faa06f4f3fdb5a2c" },
                { "az", "846784ed9483b0f277c930727e1ede1359f9b816dd01e52e2ba4012c26e7f11baf7895454fb95609bba5d93336d193241a9de1fe51b538ea14ec404044c16620" },
                { "be", "b8144a66d225bf8af73fb1f96792338c7e21335669d7287325d6591470bbfc9d38c15b994287fd247ebb9223b5848a8da5e2f8d80bf57d118369bf5d98230ffd" },
                { "bg", "9b7992d53c94c24d19a87049757ff2bfb060261658e874cf07c057a850763f640165b16593541dc2f2609bb26041d20822aea4e90b27e0a51f178ee07de8e12e" },
                { "bn", "1cbdfa3bb0c965ed63ba39e9f3a6cb03ecb56b5f5c00af3fd606d3d23fc24e62faa5f01b6c28a5967a00099b1549985fc1ba03bc2d4eb9f609d17d81c995d4f2" },
                { "br", "af12dd2a3b291859aa6d69e757e112fb6fbfc91b2487cc5a841375937a9c9c2740ce0ff540c60b9d86e58c75333dbcdaa887b4bf27b56575ee91f4577ffbb6d4" },
                { "bs", "12d6ce0ddf03f6ffcc8186c828cf31e54227586596105cdbc586bd3e477b7ca0fbab430973f31efa4db43279746810911dba94a72dd012441ffd6a1eaab20a8e" },
                { "ca", "c5334b1df03e5db7772342f0d65c0be37aadd4c5b54a15b91acd115c60d5e305fcbfaf3fe74ec078c859aa8ff07379aa8b50d65f21034aeaf34d61c9afd4d7ac" },
                { "cak", "6f9820c0f5d50fcd0d59d881ac71d62cf7221aa0d55e3c9978d869857ee26e36f9fc7f55acbdfecda70be93a8151e499c14c00338a418c1190c42b4f909d5b1a" },
                { "cs", "8e8ef3ead79b29159709d1641818158cc16189e15bae3e0145ca3daa1e93e5e76f343866c2610dcae93f3badb07fbb7445c8ad2ddec54b443a60343b8a037e83" },
                { "cy", "07e444adbac837dff50be7160bcefd57a729fa726c02c0d56e4a11c588d3327a6e619181f0f5448ac1d26f55e272e6f1332f23d759cdb13a3ba9c21166cc4541" },
                { "da", "27b8e82ee5f88605b403a89f15a97affe98d9423dbe587b0de18e78275c4ad0223a73a043927a57cf087a50a365b1527ae7fc3379d53506027a468defac8ceac" },
                { "de", "2e1222d997e66421b866502b51ffdea60476ad986000369faf788c7f608078efcafac8f1692d5cf576584b432afd3ca93057eb6756082f694f38dcce0c7bc8a9" },
                { "dsb", "61e20aa83d30926b4167f5a52f152432a5b93a12fcbe2ca04cf95bf2fa7362df9ba7c1a022f167fdc1efae8ed9c45456848d4b845556023c7d92cb19c43aae1c" },
                { "el", "43e1061ce828b05e204fdb5c26a560f6e6ead2a7df7946d0d09dbf686e424eb7695189f0ee48f000802f0e06e7c08eada9e95e892c6c0bbdf75503660d4983b9" },
                { "en-CA", "052c5b081133d2e556c8f71d1e6a5b03e845433b14f824b564e9b424c64191cd6e36b6483d0b17114bf6d866fabac8629cc4e565fe14b2e39706288a29ffa767" },
                { "en-GB", "111fc526a487767040134456d6f297377218931dabc1d150abac16acba12ecd04d690f3f474b36f1825b3f37c917f2c2b750447f45dcba80fdb89f58e1ae3d9c" },
                { "en-US", "f511396c46afe8f8390e2ced2b99ae83b5162adc2825bb39dfec3fc17ad669303003273ad2da33c0ed90b3a592b41a6146113c25871bafe86054bd11bc25d11c" },
                { "eo", "c9b67448ba211b54f397eb5b343b3bc300d53fe97c2c11c4fa8b7c15765881526362128cdd2d254577f1a6e7c4ff0408b7f6f97d7cccac6f828fd7c7a812b641" },
                { "es-AR", "58c87559e5721fd6ac7c5676e0ddebf19e866af6a5f93bb6868b5f515ad6d7d43f12f527075160e5be9f6d86cd9f8d01c5db7d9d8349ed5474fb3cff0fafe7cb" },
                { "es-CL", "e526d63d27399ba0a69eea545aa8aba5107ffaf0b16a67d20e7e4481fce50e4a6deb7750471f32853b06041f8d2448515ce9263a5cbd8461cef7d9e6eccd2d0a" },
                { "es-ES", "fc1a8f89ffc2f99af6081cbb949695a3d98428d1ff06e20cce0da59b74d7787ac2be3d8c00736d625a868be76d6c79d6ff28798c75693c369e84987bffaaed27" },
                { "es-MX", "e1953cca2989710c5bcf39d1c008a3cfbc6ed52a6d233971be5e4316da11905fc2c34b8b32f136dfefbc2bc5084d62f7e315f65e65b9edd2cfc534c3b9a84bde" },
                { "et", "17686a6e657f552e62e3389a685db7d769a9648e4b163db4458e70c066ec70a3b05cdb48372f5ce51f83b6e06fcd351d10c22557a709afa636910df95829d8ce" },
                { "eu", "3889567001ca13b2af19b338f0a431252234ff2d503ddc94f133a70874484642d5bc44fa1fee6c9736b3022e23ea9525142c191df277ed00ee8dc15ba3b46745" },
                { "fa", "dd583ddbf28fc1785498c654a9ce3a48e095f7a056be9e910df668690cc9bc9b12abc2c36476d09f7a586946313c25734bd6670002dd170faa01d3a942bcead1" },
                { "ff", "92ef3b6d6db8644cc72cfb9ddb141733cc437191d802e90fa8a06b747c2694c7c10c4a524693a81f47671001ed96d7c377d525a12683d7a4b4e067c063817723" },
                { "fi", "70ee0bd1f1cd2d457bd771df1f405ceb4996549435c901f2ad535fc1449cb8be11ea19f245e3e4726d6a53ffefba7eb5b20f833f747e42abc9c524d70e897a20" },
                { "fr", "e0bcc9b3f54a32150f612b15b2fb5bf51c1d89ec4a9036b0a231e9ee1dd80778e26c790f1eac3f3aa0eb083a1fe2508af30e38aa5047c24713adfec9a150908a" },
                { "fur", "c93c94023881fac778d921b5b39d7ac1fb24e6532a5b9cc7d86a7da1840c97541156860598afd34684f86d47f2b9fb13378015db517fd5ae288e25ef13036a87" },
                { "fy-NL", "d6bb859655032317da11c3446370ba8a85178a731d03417e03fcd385feeba6f5327dff4e93fde56deedb6f1ca3d0eb22ad7252fb321dfbf135b6d831149d7965" },
                { "ga-IE", "cba617d66c08e6c3b2a64b0b0a7fb26d98822948c9de82ae628c0629551a2c17dd82bf96e5ef3e627782674a1066db9ec277127dfa2801aba2aa86d4a9ea2052" },
                { "gd", "484b7d530418d68b9eff5813b91ea28bf07d8935fd5e74ac537c99ed6d31db113cddd073b1b3ef88545ede0dc2accb4ae428d1c31bb659bfa2141263221b2c50" },
                { "gl", "2d20d72f5031568dc2a4bf2759f824349c934e7408535c6d7ef4e866d9e45ce5609e1dc34cdbc3b6b1d6fdebeb25c491f9c7ef0efb023f1dba069d356821f979" },
                { "gn", "2f85efe1311291dde9d6c676ee864a87b1e7747819dcd513643dc9373671293960a9f8ab0ac9808788b34e9ff2ebb019a62db8b69410c031cd6eeec1f3683c49" },
                { "gu-IN", "fdee350783146031fb4e53c0a318a4344e41a3ec85c08c626fe27cc39a2de9691e7499d66ea970521c748a34d3855edc9d4f0551c54eab2f70c625a03b755aec" },
                { "he", "a715593fd9355171c5b2a535cb56dc7b707f2810fd0dd2978429b494b5c06aa1808295686d69949c32f25f5a40d4bb457bc360b18950f585ae5d91a172f74301" },
                { "hi-IN", "9262e44be114a0962b633565d08e4cba3e11127c03f0f920b7b8b1553280c5423610f02917101c232501c40c5826e60d4eaa7a885c87710f71fdc06feb0cf933" },
                { "hr", "636fee4beed51523b19c05bcb10fa3c6cce66bc4ea04b95910c037b8d827f4f41f20b5dd2919283d23ec7376c93f86c1048659cbba6240b315ec032b4ea7f4bc" },
                { "hsb", "5e2c81729cc709c0d177aa7395974d59cb797909fc1f0222b782802974094a621c1cf11a0fbc40df0a1b9187cc4c941333e44d66125f03deae35575b30965a40" },
                { "hu", "81b39f3c15ef00177f69819d51914a058384b47f05a22633f120104c6087c086ee46ab1c6564175916ede8cf76b0780c18bc17a52e73bd5b44bf1ad35aafca7b" },
                { "hy-AM", "fe307eeb6063c570d9b5a26fb9877381fb3e7cba6e83b4d9b698996131334ce6c5cc46aa1b78a15cfde85c1f1d5eca7d0ec4bc64c1487ad32fc4d9e64371438e" },
                { "ia", "bb2407ad02d36203d6d32ded55a02dafb99abe819656122b16cf7b9e1c8613b4142917579d13b2a9ac3e9faed20b6dd70c3a00cc71d83555f9b6e4a6d69ad13a" },
                { "id", "950fa2892ac373c4d78b16fb8443066de283456179f168016c359a912412a9efcfdc998eae2feb0ebd525ea320424fd695410c8fbb11d2431b60b8869787a888" },
                { "is", "3f9c799d0d359eeeafaa73d9d1b13bea8134a940b3a4ae8389bb0a6ef465289cef4b806e7bc0f513204788ea5c3aebbfe066928a023363cec1688f1605845fa6" },
                { "it", "6a1290b0165c8df61866c9c6394344a50d4e4fe602abebe6da8eeda6975b7e26c500da85144d1cadfcd4403ceb01a488fe16ac25393b3a56bf48bd6a4536c024" },
                { "ja", "6769783f9f552e5b4836731a70fa97dfe3c52588500eaccab9a77568fe638dcca5908092da53181ed9106dc92a067bbbddee023091eb1da9092769abce3d74b3" },
                { "ka", "c34d9c5e4f60e65d4b97a74f9a8c1bc451b4f4709eb0299dc83c24e07545e5117426ac746cbca9ca23ca577232e439a90a6af2beda9125370398d69c7f35350b" },
                { "kab", "8a324e25f7a158dc1f750344eea6c2d3c6c14f5740c973382b3122a9bf438e6cf7ed31cc4942120658d68bb66d62070b27983822f9932de388cc923c15805856" },
                { "kk", "911d73e6f2e02682c7f48e71cc5a38ab4190d67e21cf88c45338f34887d9e013373965247ea93456bc7a68c734c6e0b223a11d21c40a648f581981e81406bc8e" },
                { "km", "736b1806f51d216ee9f10e8fccad0ea6fe5358eb01088ec31fae03b38d16498073088f2498952cf7896ed812477d77b5c7773e16e62994ac3c770896148dd6b9" },
                { "kn", "f94816ae6346b1026f7302e6e7a839de83cba3a9d491a6d5450b64eb39bc2a2287228ff21ac359f8a038f788b56b0fc142c992e631baf8ff1065d80ccde89425" },
                { "ko", "b3de4b0fcc4217b10da0c6eabebb93736c50c0d8bd2b6e5f515ea73731a4c56a5ede151fc38569aae04e087558d4c2637c9b6c3cd4e36d88a71543071cde8938" },
                { "lij", "810f7db6c9febc33954241dc77ea91ff344ad62dc1c0cc38c107cb40c21532919fe68c266989148d5f8f19cbabfed827a1cb58d4a66e2e9cb2ff04fc60067f4a" },
                { "lt", "6ab8876400691e0fc432a8f682c3b561dae3eb272310c05c91764422d40e0f57295957c448f41876370033dc0bea8b075b0e09204c33e7b19a6457e57d584547" },
                { "lv", "ca1784ab3b18286cac2ce72efeaa7c69f7b74ff8a4e9cb5382e21b9eb333b37ec537a849f8bb864c2785d1ab37d3afe84d54459f2255f7b27034791b4e98122f" },
                { "mk", "3042f1cfd69006081171a5692ceeaa7a39cc4897954770bca404d1b16c55317ac9699c2cf5e63e693a95bf3118645b733f7f84992d901fe28d420e63310e0a00" },
                { "mr", "a9603002fcd6ec41a30a35485333bb93ae938e0479f50bc08479def4f8f415c1d444db6a587512cf59173869bac98d4917cce9faba239af2d174787d1da1cfd6" },
                { "ms", "e568a6ff10ef4ee98b4ed530b319ca681d3be338ca05ab66c89ed5c00d053f9cc2e4b9a6f79a1097ab20d78a78e21f82e41b0c6f0a1e59657fbaed5ad92e9be0" },
                { "my", "62c0ea79e3066cfea372cc87437f166d6fd630816595e2c8e32df20b0346b2d40c53ad2ab3a40836dfb8b4f4ff5c09f2942d80b444abd452e9b2e58d6c4617d3" },
                { "nb-NO", "a1431ba9189c84d32f478ece880b173ec99e59a60a0edacb5dff519f0090b26ed0c79ea64d29de3847e38d528cca0c495406c303ac506ba7c67f3a1096237fac" },
                { "ne-NP", "5770607af8166daf41392561302b33d072a89e33e1b9e4fa7641f917993ec0c05eec45695ad7982d675644104e56c249807d67abc8a6964feedcec308b894496" },
                { "nl", "6a54ff391124df40f43e562c386e1dc9834bdb245aa73a8e69d257ced3c7f79c1c565682dc3a78a2c0ec9c9ad55496e56b8a243b197617e4a2ecf710c77de4d7" },
                { "nn-NO", "3b66311677b9becf5fed0c2f8f4d4e87980e9a05b091faab5f58a6f905092612705fd9225f673fcd65390b166876f34c6a992113bb2fa5fc65338734bed9511f" },
                { "oc", "59cb0499097257d7798bea88800be95d73a3f0f9f3c73c95562fb02aaaa49a36ab97b81b04686fa75bfcf904cb34b639c70119d15e7d65b052ea3c52202de53b" },
                { "pa-IN", "d47c8feffa594a9e8f627e00529ebac592697b01911f781aae0bb388d1c3dc5c39b22c6bf9363031fe9f3d6f3b4e188dcc67e4a7c08965077743f52dbef8e047" },
                { "pl", "e3f35da19e6db4aa3e35b80599377453d48ca9b3dcfd8a1e423034f94f95e3b62cad5d0e27f8909f4e4820a97954e80a1e5304409d99be7ee57bb5fd13baa078" },
                { "pt-BR", "755cc2a9cd662b9a5a1ab81a7b0dd77d0f44f7d993e4769e38d42fc324bc5aadcec1d078bb4f6b0215e0a521b8ee2b23d13aa1c8961be8472389a16432f8cbc3" },
                { "pt-PT", "91b56b369f9cee3f0adc99a5ea90185270f302568110a563583eae5c176a278bea7bdf36d8260b0e869cfce16b252fe1838a9bba8a752ccb5bb514d01091bdef" },
                { "rm", "2382975c65cbc5160724741c7b1d1a7a3de56e84203a354b61b35d9deda939d49a790b8433f16d74a06e298b133f9e8a885b3c17fdeb4ed7175f93e60ed09c4f" },
                { "ro", "9a66fd4f7580223d5d36619c448f1bd665f9543400e76f85713cde2bda29abccb52162ff9dc86a2f065d3378abf9ea1ff00a178319d1837982fec2fbf31398ff" },
                { "ru", "a43a20c39f1a2a773791adee1fad8d10223628c5e1b7cbf08124be697c056534da39544e297ed533d7538ae431cc13598dea4346e4840bb509eaf002ce9be635" },
                { "sat", "2a30a1efa573b1440273ea8d8bb8a6e8fa177dcc02f88ec9f7360cea9b49b986cd532a9043e5c931667e07ea0a51ab89154a033ee214f249dac99eff3a830e9f" },
                { "sc", "951c6bff6969c8998b30f65f945ec69118ad15377035c0fc87aae030affe8b055b1f3b2b7edfa96db444c744a628aa4f07036836c0a8928cab3e17c111879403" },
                { "sco", "cc12f46ef3c3d62578bc667c9aa358af985477a2b3e5640773da6b9eae78ddaa0f03445308fa47f772a924813cbfa965604c89c4a47ff2fd5d8329d4f143a3eb" },
                { "si", "7617a385c347f75e793a537ccad7ea4ff0e3ada602f4c925ab1b778601d0fa37ec1559a225942ba3652eaa805d9a6fafee2b82bab7df2403f34d1b9db328f3be" },
                { "sk", "2014972bfeca6697f6745abe33dd405e43cb5c9c53963e86899773949301ffb39cd919dcf779f124e91dedd38173b417022096773a9568b77761767f1bf2bf98" },
                { "sl", "bee93f006adcb9163e7d49b4123ac6d9f3cb50f60405120307ba416f8110ad270a6701d4a125cfae7543358192bdfea0454acc8f4022ed5b9579bf302b6c35c9" },
                { "son", "650d30476b8e3ae08cef6c3b98e83f9822529ffd957443c4304586eea854f64270cb5a3f970e66b1000f15b0c08a6a29206a01aef24936875e0cb0b770f47686" },
                { "sq", "c20400da2f02767f9db84b1403f58ce0321dcb2e81dbb27ac70adc71114366ba4d90ae2aa0e47967771cc0eff6b2bc9e9c12219b4010d90dbce130e22cae00b3" },
                { "sr", "ee94feee22e8122cb56e8f42e6d0c0b4d903348235321e21247074cd12c5c681e27c15487b7553bc21048ccc4cbf9fb34fcb9af1f2da531fce90494d00a2327a" },
                { "sv-SE", "b15baaa7ef5dfbc2b81b5187b7536a445e3af9ef7dcc390ebf304af003215f981ea82ade318d6414bee194e38cc290d52a9023975352f1cfbddf6a56943891e2" },
                { "szl", "d8fd011462b1f15a5fbc027fd953386954d1d48605271967dadc7ac41166d88054b9eb8c070966dcf03ca66200280f7353aa1b365f76e88423fa0fa56a8bd5e3" },
                { "ta", "2761d71624b44bbc00fb89803da210c47a994848cd0d2318cf789e95c3e3253223f4dce334e080242cda56c793f53532a8f1c73b79b9836cb16b6284489be0a0" },
                { "te", "668f858deb08c8b0b3da2d258a2af0519905e738fb8803580e6e2d2cefd7dcce7b6fdfafa0bd342bc74dec534e609b05bf616d104bae75372f73cc12659afb17" },
                { "tg", "deba06e78eec8319457804e46a7b0c29116b7af74f1d3755a7ad5a4e2245518cbd786c5ee74a09e7a5c4ac0ad2ead3374a8322950d459fd6e442928d7723069c" },
                { "th", "06bbd55527af0241a7db0bba5ab2b414b4814f1db26db4c46305ea4fa791c42df8d9b60d8850265e5d3232ff87c0934bdce9e35915b36e7dbea2505c474f03b1" },
                { "tl", "c29ce4d150e5d6bb2dc1a56d5d59184ec045fe33f91d3b07aeca01154bf17f1ec5ec74d5ea31d0ef5883fb7b4c8f1ab0f18100de38e7a4f381d6811814cab6c1" },
                { "tr", "8d30be114b4060beb7b50dfb6ef77ba821e02cde8228ef46e07a78c1d5f9ed647731d03681d8bb7e59eb82fd252939e88ff68b9e8d3802ae4c3b065990e30a87" },
                { "trs", "9d054938865f14ac84a9a4a8cca2bab1dd90093cbf3160ece6fe573aa64cf4b3ebeb3f606b32cfce7afa1e7f9402be1c1c0b51468cc109cec4919dc2112610b3" },
                { "uk", "19ff8f946fe15fc72006f89f031f56c83b7e925be7ece03b8b39b03e1cc7efb930bc8c0fee0ee0f21853d27aee1d10d53f6af078e75b318401a951a86b3f9351" },
                { "ur", "989d1f3eda42bdd5bb0a6f43b75884e73477b59894cf623e08850d92b70e3fc93d00f68bf0bff0c59691b31f782f0befee44064b5f220fbf1c6ea45d3812331d" },
                { "uz", "581b17558deef2ec10cf87bcf332e17c6f733cfbdb226123b85ac70dcfa5d42f5f5b487c377c11ea3cded8d085baf418a48f69352309ae2c78022864caabae3b" },
                { "vi", "deb5ce04929de0ecdb795ac0f3f415d1286203a39f1145400a191c76e180413a9fdbf31abe7bef370fdcc4ba7b8d6225b24169f02bfaeb8fd09b5cff48620cf6" },
                { "xh", "47b261bedec6e5422a6cca7812922d95a5196a049c9dcde9817f592dc6f92ba68e9c61b060fbeb013c40ec1325661090bd14e7009c5decb997772c62dc2e43b0" },
                { "zh-CN", "6971759d8065faa3be9042ace3335fc23d69e2c66c36006367ab1182feaf5d31189c3177c8881d68838887576f18e5e6e521888a031d190c0d42f7b702c69cb2" },
                { "zh-TW", "87014c915d912ad2b2eec7bd7ded6829c0a154aa2fcb2dbb9bd9e1161a33807f6006ba9c3b7646f4060961c076f20dc072e8f4da2fd5ea385b64e50548503d8f" }
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
        public static string determineNewestVersion()
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
