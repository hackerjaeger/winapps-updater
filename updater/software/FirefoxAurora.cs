﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020  Dirk Stolle

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
using System.Net;
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
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "84.0b3";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
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
            // https://ftp.mozilla.org/pub/devedition/releases/84.0b3/SHA512SUMS
            return new Dictionary<string, string>(95)
            {
                { "ach", "4aa346a506cd2372ad09f29ba4c8d993d11b1532d9deaf75ef395c236198147b039ffae40de3fe1d5ae1414deaac333d4379c4ab86778fb11902ee051224d670" },
                { "af", "b3363320ad0ac2018f2cfdaa130dd0cf39d8822c3abe6602499f9fdb24a26cfcdb8cd1b0230617395963903ae17f467a4f89599ce791e9e1f0496010466e6d65" },
                { "an", "369c5f6e9cf38a0b9e5bf38abfbfa0ac370ba49896cd0d7908ed44f314228c02c536d271200f259fa2a12661ad5e28dbf11b66e0ef0b88a8892ea968c86653ed" },
                { "ar", "be514f0b59ff7ecd2071c1397b488d52a0399940ae818e15768f961711fadec60c3c76e06edb7e8856e5342a485b6275019ac2aae441ccaa0948bab249b5883f" },
                { "ast", "2375ed3d34971c752be000114a8a941728f84a5e64f6d6313028181ca60d688fb90d60cd6a8a90e96c5203fc0be9657d1fa7dca384b56ae77ae21cbfdd70d72c" },
                { "az", "b5e9eeb500c4941baa8bdad45c988f7ef36480761c9548b388d9b879817bbe88d5078c0879bb4f6742a369f879947823143711d9422b43b1fe8a1c0d2fa8947c" },
                { "be", "0bc7fc83c28206a5b79c95b4d1d0b014b977aa724a02d4ba2a4ace42aada36d671283ba36c86007dbdc82da878e9ab615e37bd46d50aae0d1b85a12997eea8de" },
                { "bg", "0d1b702ba7a1b82f756a2e5934eb1140bb9a638c7ccde5825b6e5fff6011cee73fc65294533dbdf5e0fa38d3e308cd1a5eb86b35ced03f0b403cacc4fb1f7e7c" },
                { "bn", "37f47724fdc47961dbd34ba1d1947af1323531505467cdb8a1d83fca00260a96d87a4b9ea8767ed61a352c688c91c93d77957b213fcfa2013f7a992b173315c0" },
                { "br", "3d0a583fdf8caaf9fb53b7e65dc69b6166401bb78af6b870a601a9c5cd015a03bb8d2f39f2886e8ae0240b354a448dd85a2f288398f2210f2ade00322d8243f3" },
                { "bs", "ec0d58cdb68d273867f2aed0b82800eb805af7d26ad2109a1a8e1650607005da593fdc7c260fd825094419f38dca83856f5fc33294cf9eb784e2995337fa54fa" },
                { "ca", "e58a5e09ce59188e814f7619191993e65ffe3799d8d10edf7ff235761bcd984940eba5d5f2ecbdaa3d860d727a999460527633b12117757633a8f91ac47c5a08" },
                { "cak", "f08e862f618eadd3fccd8bcdafa86459983d293384dd8c1845f3b3b75c05b5499f5c9020b1f697bfb20e06f8102e7ba97c550565577ac0e588611d9a4e15241a" },
                { "cs", "60c68ddad62153c84a473f8d7be6167735dd05fff71a97a75aeda7ab86f538552581448985a52017e510e8d3d01d2c37194193c28c1311ed6b3d94dd8e008ac4" },
                { "cy", "6d9d46ae6302ed36123908168ce54016f61cbbfb7ce46104b3fc369c970dda2d90580880ee3753b3c46b4307502700eef8929880fb171ce165491303324674a2" },
                { "da", "37bdfebc51dc0e5dd02e2c1386fc45b7392e34a529301d641f58c591f90fcee361b83cef7636ee6a27831dbf120d543269037623d4dd24c6e8af10dc76e5dc76" },
                { "de", "11d57b5b34f7d05a9e3a51d9d70ff91c8f8f38875ab8ed3a5702deebf470890c582cd04844528fea2d3a7feb4f81e03afe4fc647768198394ff96a0d0ff36ed7" },
                { "dsb", "33209ebea9c2694a5e01ff4c6e95c8cd8ae96ba95946187845a59470e251a382f600b0539c297fed41edc1d5e993d4988c833c43e3afddd98a6c6708ce504e74" },
                { "el", "3de1eb7028415d501075d2d6419abf4d1c6a1e329299a116d7254c70d5dc4f776a2864decf5e9227487f5e0f3306739202998961901875addcbe4827a5272d57" },
                { "en-CA", "adb07c49085f4f15e5ed48b9eeeffeff9106be3016009c187ae33f2fef7d48f6c744997ec371474779b48c43ee91ccd24fef8c68f2841fc473aa9852f2ffc458" },
                { "en-GB", "367803302c76637c916e0475eb42a4cd0bed07db3a743b0050a9781db766af788d53e4d1a675fb764f2be4982b0ca486165e55bd1dd9c907eb0f9f4cad44585b" },
                { "en-US", "fb6bebabd8f6722773fa40789ed13fd50e14054b6a7132a09763b318b6a073287d06e9c8f71c0484dda4adddaba3d42d809c37a54b5bd36fbed7588f2efb3f6b" },
                { "eo", "fc6c2b8cf2b7b9e0a65b49f537c28d96bd0b0cceb8d2b60f7ee15f9aa392d1153c63c3140839335434abae06727649e31d81a18b0eedf9d40255230b17c25279" },
                { "es-AR", "ab583cca3b665af39c40a84cd0ff7fa61b1eb1edd11274abbf49f3276b860c92eef839c51f7fdfb4d2a6c233a854ccb0c82480fe96ccb691a6a176426670b301" },
                { "es-CL", "997ebbf31de75a489951ff8be4405efddfd0b9de8cafb2028a2059c4fefb79c5a78f66b5506a368081382a1d4c9a569528e5eecf7a7218509783bdb9b4d89c4e" },
                { "es-ES", "8aa3ee0da5c1dc576489b08296e1fa9a558d83042de483a21491fcf4de6345d2b7d4b1e20f4d1acd8afbf119e1b1ecce6181911746030b66605aa2e4a6c6e56c" },
                { "es-MX", "cabcb2fc49567f19f4de60ec1bffcda116286be1144d09b7a0181959e9ee3476d55d3645ed68576afd5fe4cea512d91d84221066c3f6fc61d4de78b7020ea42a" },
                { "et", "f3d1d2d20c0917f5ff97082154215dab85fbc600462373a7bb55546fd482595192c12b9e898830cb753096b557c0427bef16cbd4959d26242711373b34637700" },
                { "eu", "f95cf74b7412221db5ed137999d40fe611a691a8ab64b22b290268500128c8077b21f0df8c4a64ba9e03c51eeb62251d8fba5c40339b2772b5b16e83b75ce093" },
                { "fa", "2871ff3578d413bc7630e2cd69646fc7d37b0e83f7029790627816ec4b54b8a0ecd5e781fc08cbac8a98c74d1cd1306814df9aec8efa54759c57cfbade17a2bb" },
                { "ff", "98093b81913a9aa4809b000afcb8011c6bde622c71abbdd9ce622a9bac2252b8900ae171e7a21da89fdc935e60d998ee6ea70e2271e12ef3ad111604c76397c6" },
                { "fi", "8d36b2939998e7ba8ddde7998c6775775cff5e8f7747031775659b512c5d72d59bbcc4e80f6ddee00d2cd6ec2f5a86febcb482ff847970d699297068a4b13bb3" },
                { "fr", "f840cfad6330488ac924ee59e7584b099e56e2bc4785b70751cbb0ad3b5e6d7678b21804baa711893e69f3693bf2535fbea566144102f7c75064ef0bcf745c0a" },
                { "fy-NL", "6f712f8ee9e21a7828b1e39efaba3ba40cf07600cf2145a96b46ac5edf06b30c6bc2d6409c589abb754724699d06c49d2e9475113c5bbad966545c17a2e6563c" },
                { "ga-IE", "7333e58d06fa1e24635f98233889094cb0ae72af03ff59298527ff72e85ea8741b5965e2d70ae4a47e33d831b8413cae90438c851293a04509697410d0f1bc16" },
                { "gd", "94a45e7d26d5e11ec7371fd243a5ef7f73ca45b39d66f64c9e7bfa2ffdebb72c8d70a5d930ebcf513b261eb6230aa1cdc40a8ac97ef3116e96d292d494252357" },
                { "gl", "9853adc77f93e5cfc9b5a136eee31ab44a0a4653d6c0bb39d282366d24efa7c494bd7bd954a0c5fd07c98e27ac4406ccedde6d379bf8cd7c9b047da082be1985" },
                { "gn", "d96138334df323aa00e6966166c712703d9fe121c8f20a1a131311541362ea71a1a3e7d8511c9c8beba22136c3b38138cc797d155484d34ad308dd3ade6a89b4" },
                { "gu-IN", "00279789f4c2e08ea2c3cfeb307a927c4d32802e7b270509b6230c9062cbaafcbc4b184f33321c1238508af1e208a84637549b83434f40eec70ac25dcaaaf053" },
                { "he", "119fc98b95098f39e4658f5a7b2711c0994d2a8dcdeb4ec9e036b2a9859cc26b13d43c779cb8d651e67241882e00ff7043c1c5fd9d60e39761ca87c0d09e9cab" },
                { "hi-IN", "ac33bbb1022afc02739f3c24f841384a59e25aa0c9222dcb5d3d8cb64ea9b60094107560695023e215742e9b024a2adaa2426fef9a432f432e3b2cb5572d1c13" },
                { "hr", "19563982ddb6c8e4f1a824524e1e83d5a60cc20ba28ce09edfd25c0284196190a2fbb43d43e8b4d1b75c27b1c75c551e35ebdded3415be03295b69e6d39a3141" },
                { "hsb", "893986171e1394e3bf3dda36c14d165a7eb9d95f2aa8d8101751bd9c2085015cadd2fd2dbf866696cee425cf7920de88e54cb010e4e5e758aed5ea8d65f4efeb" },
                { "hu", "7ecd7bbf5f5a8ca8edbe3ba8d510051fbed991269fa8d8fcd869e8a9b3e4031071acb422ad7f17dc52ae55103b3dda632b76dd08ef180179a9813fe73cb66556" },
                { "hy-AM", "73a5b50b7756f78ce0fe4cc56858e158966aa867ad5ee8ea938eddff6d90c6a56fc335d38956c028fce0d207c5cc5af57af343b8051bcb3c07dfe0f6cd882242" },
                { "ia", "e3324933aca01893ca421c4d7a62881f8054ae16a5d6543c03a0642120ddbf8aa33782822e5978cf04bd78f090b3ff4ea98917c8833f3542e553f942e1e90aeb" },
                { "id", "024b3187030da350180fb364d677a646dd78df30631eb00ff11274d5a99e312710d6e379ef05b6df834570200ccd3ddb6167817ba647c1fadd4c4505c39161f1" },
                { "is", "d2fc7f03ba8de1e99ff8988200aded1d75fbe64136fae4b36d67ac40ff725a7be7658ea5ae44b4285a440198df0b8f108866d56d70bb716af37dbd1c365109eb" },
                { "it", "d5dbd49ec080fd9e0439c521d63dbc9f801f315258e6ad271b319832287cd9d1ea5db4fa6c114a457c59f712cbeeb3ea6b57c525eab655dba6247fe089e75cfd" },
                { "ja", "3cf01ca50798b69e7e859c015c27ad176679a68ca849048a6129e14fade938ddb30a8daf36076ca8abea48efb8479465b215b170986c19a21f04a9403547f08e" },
                { "ka", "7d99c0d421a38c7ab6fc78b33989bdff262bf0b9b9547ac9e8d7e12cc9f6594b5052716216f4c9776236bf4745813482dece7c09933504488e835cda917b1309" },
                { "kab", "78f386d0e9a996e89145f0c271179bf933cac93e2d2071be1192c8c809783d1c958675b7fc92f301cbf8290207145e7d453bcd52daf54078f62b8a500e4cb273" },
                { "kk", "353d831e518073a8a1a9e92fbaaf27f0cd821b72a609ebfdc6f9ce609dfda53183ee7f68c812c540f6674168da92c760c91ed2632fd0e1421c463474d50d7560" },
                { "km", "27f331e9f61d6534d3878ba5cc31f5a07f67b821066039cc1e1b6b256bac4fd52cd78e63a7c727828018cf30ef8098c0bd212cc1f6dcecf0148a73e28a4a8dae" },
                { "kn", "6a6960b0ba21587fe6e3a59a0f6a2353f814db516260a71579692f30a752b2addba20c411c0172ea052868a15883e541403eb417c177529893e512d480329802" },
                { "ko", "36130ac3c0e5d934bdd4140aab823c1bb974b11ea862f98613f76ac6a5c05b3b0e04bdc92f6a8f0b807e136a583e26e82554536e4e99d3d55f636e5f9719949d" },
                { "lij", "9999a086f781b7c5e35e4c2c441e53deddcc7459320691a4bcaccab6787118077bb1e676c43432de319db515fb4d646e8829932dbe0d2262ba16c57381937469" },
                { "lt", "b00ded3a8698d82132087122106177937269e44fdd74210471bc4007f7e1b608a07be4024250b83b120ef10aabebcac316d5665dff2bd54093f4a61f271a58b6" },
                { "lv", "153bc75e855e57bfc70c9bac0a4a773d32c5210fd69a41259476d74e7fc76093c64f10ab2f48e824d2e9ce4cab75ad21747984f2a7149da07b07a2e6a138b4b8" },
                { "mk", "ad0903de711f356bab63ad83615d2133590ccd9a9c2e48536468512fc82c013362c84767b645c44e67956a0e2783040fd0f15f918f3d5b45729a547358243310" },
                { "mr", "ff8ecebe8936f5e9881cfd9f35b5950745380fa6b9dafb1c67c828edf81bcba8d390fdd5ebe5aa8366323d43153e0e57ec36a0b3940eb99f6e923e8fb810b194" },
                { "ms", "8b21948b36d23ef4a4a9db9f564a5b106f18ac09fb741066d15341139bbccb4446817307ff7a66355c7c970b7d74155744b238648c7dcbb3e03f85aa1aa85b5f" },
                { "my", "b76598e960f8cdbfb09cf121743b8a0ef76410d577e539e4acefee2e0e60b392fc7170778a1e4aa8a005637db0166ab91d1b27380552fc1c542ec4fdbf5bd41b" },
                { "nb-NO", "e9c1e592fdfd20d78c74058af20f2cacafe41ebb2ed33a817bab1a40c4f32b9c66a9a571df5169f7acf5d14e9f731df0e67c8b0a849d8a03a789310b4ba3da5d" },
                { "ne-NP", "5e4bff757c05ba73606e3db255edfbf9c4d1d5e48dba087ee33db3128ffd818b7a45a18c25cec5f706b499ae924e9c20a3a2e5a00c54e202b482ef0540a4875b" },
                { "nl", "ae8ab8531e590dbe88fa9a095c509af67125c6473cc022f3b941065a22817f544460afa92f1bd09aa52418b23a65396e3a83bfc3ce8168a999b8d1a24786f44a" },
                { "nn-NO", "4bf306e40bafa6a1592697f043c8f8b39a03c39bc6ae028ca2c2c9cdb6bb6eb382c4e74f25fe72a52ef1bcc6206bc5b2bcab1ce5ca860bb87b3fb9083f8de710" },
                { "oc", "604df08b6e0d337a89bf100f5ec112cf53516af635a4fea7447d3160e662facf01b53ed52dd86ac3e6c4419215e557d9aae02dd5dda475b525ae5e81ca828f33" },
                { "pa-IN", "053702edd36bbe8301f2bea13347bde14e0b7adc5bf163e3976d39417d0e0d48faf61a79419f7596248d04694293595974126b171501ad5f8c939b6b844d6e74" },
                { "pl", "7eaaacafe7f01066d14bb9f19d9b3641dc5aa5696ba1e9e9766330aca6b14cfdb4a34b9c56ab11aee5c9c45bf5c81ff185a8e119b679c76c74c1c2bfd36ee58c" },
                { "pt-BR", "ad42c325f7db9952d4bf7e4354a808cb69baebb21c58d158450a00167203f2106d79da404d148c88eb36d6de2c96255a8cf8df4a0d2d28de95fd7cdc197db9d9" },
                { "pt-PT", "a759887206d13c35bc159cfaa97fea2af1070ebe6324422cdfa733b67447291b34325c7701fc11293eb7029fb433e65a4bd7693741f50e58e0f71b55067ef9bd" },
                { "rm", "391eae7b0b6f8ce9d503035ba9999c121fcaea24e89fe24cc2f2e86445aad2407827d879b60e5acde21d8e1d56883d346a5373b5cc49c507a9e1a50f30670b96" },
                { "ro", "294c0930ba00a3cf7dde650155c3433123bff249daa3e05ef0b7ce375533b2d006220b96fa33088bf64be39cf467de7dd7a838253479ce9f595bedc5c47185df" },
                { "ru", "2d898dc95cebece932f17c9768ddfbf8dee5d7ee1660392f93b305410c8b1ab34879093ff8249ea629405bcda40a5195a9e9523c8449159ac335e2c9e79b05f7" },
                { "si", "a71175bf4dc2a46e5114149026c13ce1c21535716496e6d48dc284ef8f5e30aa8ba1c9d72de0aed45afa0a6a04dfc30c6d1995de940b606fe1c3afdcf46e99d3" },
                { "sk", "bdf3734c3a1ef144a4e65c07d8e9e749aa55620a1a1e78124c2a9071ed099e28083a6f0c23f2ca5eefce8aec987ad26b4fc2427fb14b1aab22563d0f1a307e30" },
                { "sl", "072e4c418558271749bc7f48fb405763f190f60c1c57c6763f6758c8753197eee471bbe0d0801aa8fd7a2cbc6b298f42fdc2826dfde680d491308bc6098b2699" },
                { "son", "2f80eae1e7230261febd7d5ef360999e3ded909db59b1812ee2ce6461d075c2eeacc08a3420ffb4ede8f4125824d30392af7016a2a1e3509d280eef74102152a" },
                { "sq", "c3d4ff64d8f0cd977980d51fe63c44ab3234c70733461c42fe8dce8952efb55d8625411347a91422bdf03c90f9c46a58bcfb6defbe89e6a01c289d81255a28fe" },
                { "sr", "764b015ab640a8e3fa6d46d63f98352c33d0ff4b1734f731fc7313bea544a31f536d7db9d7acc48f0db426fab630c6bc2b25dbdfa9951050b6795b7c2b984afd" },
                { "sv-SE", "5ce484f49e668a3d26b2e328cbfc38bac657e957c019bc40251ae3de8197c93209df100db78b7e00ccbe2a410483be47422187112b1f2b462823439ea71a8dfb" },
                { "ta", "5fd7291e66b57e1bf8620a34c4887ed8599825afe3beeb0da9edfb7ed8dd5c8ee19f56083d60da06db43227433ccaf8d9aaf644db865da253f38d640d8885580" },
                { "te", "dbe3df06bd239e2f49fe9457236ddf39055b0103906f6007cf6c55f04ed66cf5ee8564be7c39d89dbb125568300620e7cf459e4d4f9d0378a390d5cefb641738" },
                { "th", "8ecdc54f047bed2811926584be7ef61ef1ccab4e08d4e9bed12a3fc64803d0dd916a25cf3fa1555a98ccf9e632ae35e72516d313a954cc4bfd38e20a0dd0494e" },
                { "tl", "a8422933bb456ef85f213085e2a453ebe744632d27b3c0dd81d6e0a3d628ff6819824c0f57a8f820ed932fe08b5957fbe7b03e1fd48066bf70693d9bb2b8d557" },
                { "tr", "656bd7a160ffa0092d98da768e7cd08b3f643de2be4fd30dec905bae2733e1d18f926a1073cab5cd60e493f78a9181cf66fd3491aa625d3162d7f3cdb359751d" },
                { "trs", "c50276bebb694cb55fe86103ba462cc2648b3959afdbeb34a9b0ee32500ec18376188da6d44b2e8e950c88e48091170ea4114d065a288e9382a0d8d0730c4d7a" },
                { "uk", "a24babc1b104a9926ff14d681791dd9fd99e2b57cbb606d125ad8b87ae3cb3a1ead18e00f689df1f963f3db9c1ccbd17df68a5c12c5e4ba767dc800f6a0e5db2" },
                { "ur", "e76d42bad979fd77f0188d6b54e710a46245021d91b616de0aa4d1a3d5bc3b53a618ddb401d74b204133fb7424c8717020a5b42548089c307689539350fbbed1" },
                { "uz", "27f96bee192d8d5ef1a0b78be1c51e512506ece6afa05622a448038b808226e09fd6eb4be6fbb706892ad2763ad18d47fe1c600d46dd7a6d82fe9e1d195310b6" },
                { "vi", "801f9999425511658d680a42efd6edeef63f13cb0692441f80d6e3a4e08a015099f37bf57e1f055504841316a0f718ccc5606d8d5243a136b0173dfc19dd037f" },
                { "xh", "2ffc9fb7216be0b7a129c245d1c3702a2faaacfcf9684583a933d820aacd9f96e7fdebbf328ec0f583ce88ccfbee37de0c0b59ee055a3e2fb25a372174a59e8d" },
                { "zh-CN", "d15990dadd42c1c641c3535e4dcdbb7975c0bb2332a4359dddd38f1046063c72d4bd1b10b186ea303280d9975289b55689ea2a5d59336515cdbc9506a71fa5d9" },
                { "zh-TW", "95cc51b7ff08c88ff11c77e11ff8a6d7e5a0ce75914441867808a1763143adc8cd36157940d81f2996f0af208a0858e997f990ccd7917724c2310477e045467f" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/84.0b3/SHA512SUMS
            return new Dictionary<string, string>(95)
            {
                { "ach", "321c9e990129888d553853eb3b2c8bf1f4fc27f334e2569508323b202738d7be4767d2414842b57cb3b03caa252de3f22ad169d00b9ec3715b1c72e3e13be2cd" },
                { "af", "c010333e61685f1feed44431fc5bad13c677f8ea997e2bf947b6d323491155ee54e2e37d4703f67fe00769ac54e36df72a360fcab9117f9e0a75fbe75067ad2b" },
                { "an", "5c24d6750ec147f02e9d2d9b1ae57a9fe6433f6e7c8b2c2936a213a396ccf02ff902add083a6bdba88ff8fb3afae06fadcd1424c94a53c043a99b89f14b21c0a" },
                { "ar", "9b59075695b6c82d00f3bcb03183b3ab9cbc04f3e024ef8813a24d18d6d265c4cf68bcf017edc938e7a372fdae615d2df7fdb4ec3312880de9540092c7353dbc" },
                { "ast", "8b91c3035f96406fc418a7113f3bb57c88a2877f771ba7ff7473a49a45e554104c0e40ecd2decdc523014cab1367e56abe4e1b6ec3c07dbbf5be0d73369c742a" },
                { "az", "828c1d9eba29bf6a63d28594d286b00184399eedc14869032533d03eae77c766d6e40e5623407ead5af41eaa73d03d38cfe34e857c3d1188792af16cf023efd9" },
                { "be", "f543349521e6969235adaa45de05969e4f24902e2f5b3d255d1cd4e96312f13b1d7313944e46aa253e5204606e3ec98807c4c32ef96902fdf442d93c90432e1c" },
                { "bg", "e2aad87e1817fc279e4d7e8b5f19ac9c285f5723c990c8681ed14487f00a851d07f7c697a010543249504d88b4235df1a66228f2835f7bef9c9a1ad459c4c178" },
                { "bn", "6b815b9cb82ba510e938f2d450b02e541f63d3217ba9edb4468db8e186a790d7cc11bb5a0661d8fcb782ffc62bdf5195d1db7f7105c329d4586fd97d224c7949" },
                { "br", "f4ab9d4062696dc27961716f1b3c98783e2aa597d1bb02ecd0aed6f79f7a5d01954505cda50285102ad8efd158afa3c005ca5caa2ae96a60a942bc5d52c239a7" },
                { "bs", "ba4d5c51f3ed1501aa7467aa0216b427c09ecb1a14659a032b723fa412bb6536ada5f9ecbd102291023212d088464d085392300d141252c4e5a4c478b99ee411" },
                { "ca", "68e34675236f90b0d7e73bbc18b4216ea8f6071a47a6afd86215541c2b782b88a2b3c32330bb5787d78e80652fd73228986156c7dd92ed043aeadada686ca2aa" },
                { "cak", "130e919aba55631d1da850750e6252a6e9a90a1fdcd45e1b882b9753a96135eea3c9daa91a29b8f47eed96f2196f70832a566fe15450259de39aee3a9e960b14" },
                { "cs", "57f0e2caad836c423782181dadbb3b5523cf8c176c29c9cc12972ef34410ca6db4a1987ab143b20833403bb5b1670265b16844ddc96a0188fb6be2511b8f811e" },
                { "cy", "ba8c0f6f3d7ff93e583d99da397af795026ad8cd08e3adb737ce3a970d2527c354e602b47957e2de62ce54929a160addbdf00ee0a194685db2be4c8357d6e94e" },
                { "da", "2a39cc15e9005b144f3f90d5f3e04e175499de506c721a7049c36b2e3674947f5f283593c083862d73df219ef2c8abda339b4e68d39f66dd8d020557d6c41585" },
                { "de", "ac8a0869f90bfbe1c9054ccc91440007675f843315da50fad03737736e5edc0f647ae6cbcdb2392ec4d0b104927c75682b50ef6f15b08ea91476ce45c6f26a40" },
                { "dsb", "0ba419a918c4600bc34134bd6f712a3ebb311ed8758490fff14bfdeef742ed9c4298c99945f55983c6118dbc8b4fcb786124bd50a216dc31c9c06e4f4dec565c" },
                { "el", "245a3087eb36722958bab4335c6f7980be7b4dec55806c059a32bb000c160a32bfce3e55637dd6986ee6cf7e0638dbb6b48b0036d243ca45543423172e6ffa29" },
                { "en-CA", "cae97e757e60465ca1495e37851a5a8380bb053e1d700e4bde484c2a4cee6e2ac0695b40c59ed627c9bd6fbc1bd845e3880c663c93f0d8e9321dd144b20650e4" },
                { "en-GB", "b3b96e7a5063eb0528157e0a301538120387d571477e333cf14d332d2010c65d5082a50507969934416d385b341180bca886e2e657f4f448a9d91b290413ff99" },
                { "en-US", "c8d6303a25346def5e26f76a8541b92fed89e167d609da50b2439bce89fe79cb02b6de78290cc2b91081f217e870a6ffaaa45d6c40eaa7de68d5cae77f9da7dc" },
                { "eo", "c68c29f0acda5845c35e27775eea202249521e536b81c28547fe785b8656b1c6081633acfcf6078f1d35fb61eef4563851670122aa56484c2327ca6f3fbd1f0b" },
                { "es-AR", "b1925c5e4efd864a9447fb898da9ce77753c2b31f930d29aa6b171e42aeae38924bd41df238c5f0bb39b62bdf3358e4ad49676abd6a643eb15297d28d6266fa9" },
                { "es-CL", "95f1524d666d1870cc3c7d841b225f56594875bdcf741be5e19e1c5b2f9d45bbb921285f75e8f05dc5a5b58bd1872e214e607dd0d823ff024de88bfe88f4f711" },
                { "es-ES", "97236aa0c5142bd490cf12154ad6eac146fa576e95cc8fb98927b6207c72d87c27ea3704406a3527e6a9a62fb6c62c0bb095c9e17ec7bd3883ddb8bf32f441d0" },
                { "es-MX", "38d098c999b80e2efeaa4c475140160a3c3145d996f4bdc56e5331962e775745acfad9b93b31adb003a97dc68999e82a97d2668cb23ffa2906ba413b8fbd8932" },
                { "et", "5417c05f62e86f5b4f6154ceb56d321a9561f2e99e2fcd6bdcd92c7d734ff11d81a9ae37de25a6b6f5d854850841b151b2f5cb46942c62d94c94314902a768cc" },
                { "eu", "a4e508fcf0f6501459169254778fa758b605cc0ee960660648c982c6c3fc2b3694f8cad0dd8fec52a500503c78472a9753dd0f4f8dd1930c6523429c8e66a56c" },
                { "fa", "e2c6e20f017fb70aee3bf6f74ed04415f9b747dab613c82cada93c50a0aafa65133957520d8d729c0b3731aec5385d03ef4d63d45b0284a0c5f5b297f70f451f" },
                { "ff", "4486edba2cddf0ab8aff61c4be5edb6052910d45d0afdd7e01f884be227d0ecb1df6c9732bf5833349100e9103f6c8c941dc5b565a6f4a0497effba48a28d8f8" },
                { "fi", "f433989b6223ddd6909b55d208b6023be8971a1186e26940c9ddb6b4283d72e35449361cda514d9cc2714736036f1d07317f4b9cc7a4b11fd118bb885075967c" },
                { "fr", "05abb55d5137fd5b0e6d9cf8211c7d6c6f0860d0d3dcf8ebf95b70166db80e26e0dcfe08b50aa8f2866e0a7ae59a1d3874f485b33b593264990a783cd9eb2b3d" },
                { "fy-NL", "f7146b155f0605c5326b32e78a9f89f07e1a4c3aab7eb272ccf339822545a0e93831a0afa3c3462c8888036caa7e52a69f1a18e0024a27b2dcc07060dd06127f" },
                { "ga-IE", "ad282299c1a43720a108e07116accc8ba452118176b4bf0b5b6337b789cc596914c6a0614b4cbe70ce0119e1c41e2a54ac904cd0b54b8a8a686102cd54719688" },
                { "gd", "8d9f41c4e37dea9f72925616fd5b8f50bb12d32e523fe170a0a0ab4e948cff9dfa2482a276f61e48c4e1bc0da31a68af160470a40128975b1cb3a30526774e64" },
                { "gl", "2bcf1c101ef32e3f4a57d9ee3d3aac524091fd209cdae783d122829a5fc77947bd3c919ae84926faa36755e739b522ee17f5fd38990d6009a3e6b415b86091c5" },
                { "gn", "5ae573c903a6f1f2c1c280cd9cdd82f100d0181104509d14e62df38b6e9428719873c9d2491bddf6cc2ed7990a0701d50bd592c3b89ae27f6b2cb5fbf0adf681" },
                { "gu-IN", "c9d38ef50ae3c1e434278e7e56a69a2de36ee63417726eb4bb364f55da2c6306b7e2d47d2b5beffa537c1cfeac69807a86c9320a70a80455e5f3147937b5260b" },
                { "he", "cc1892605156db81397aad02320af43603bee64c50d714351c304627eb057a00f1ec58064228dbd371365b427494e585cebad3e6f09280119fc02949eb3f7f21" },
                { "hi-IN", "28e9ebfb069bf4404a8a64041c8e0c77ee7e69aea20a94265eb312be14569d60bc020e3493f9705e021e2543e2910de3eb85c4006e6c6a3786b7920f63f32d29" },
                { "hr", "78e71fa7bb4123dd2fce19e5e9ef7c08ef9008e6f0ce320a1ae48f50b8c4937df58c352ed4a5f8f458d6e1f4842c755a75c2800c210a9ae186121f98762a181e" },
                { "hsb", "e013e6b3cd7715fe7730d2ad6a8e65f766cf48fd09e7ca5f7bd599ef59de2c3aa88ff476efa6348062af8c0062c8478df5c03df3f9b3328bc0a9830f7cca1f3d" },
                { "hu", "ddaee9b43b6dca2df4195aaf723534fc589929d86874aa83fcbb303c82a056073b4290e9ad5236b0f7452caa90fda7baf874e2f4640f4736835a9fae5a42aa79" },
                { "hy-AM", "e58e9306c4d147a997f67cf21727406cda919c0daace66b2ebe9c8c0cf13d35178693db3cf21bc6e6cf4e53de434671e6b8ddb68087f1c834432f87c17f34357" },
                { "ia", "61b9f40183024a21ebcb7a1052a3f2997e49b5ef5036ff738090a1ccf7f0aeeddfcfa180380712002a28f8b95adcf384d78482c3fd6b9cb7fd2e7dcbfde9acc6" },
                { "id", "1aab7196760beb449ea34df8b00d8261d18b13cd4790dda872fa6dba00b67a2a6ede3cd446171e01fe5043a7881545d2b866165ab284469addcfe0a4fc35a03d" },
                { "is", "89d7a455b3709c3e833c0b1a106a0b7b7aca8fc6efa36fec618efbe064f572aa0876e92a705f16701db6281425fd6affc692d843772955349da9331e3ad3bb10" },
                { "it", "67fe4bca47c12e717a7b70b098891114d3425ff06fc6ce9c03486555522a56fbb3914b279cc25dd7fb50148d55677c668e746763e13f7d13f6ae83ff92baba98" },
                { "ja", "5f49a179a7d1c251f2638829c32fc7772877a5203c9884f33f79c9d45d001f21331588dc18f0b20bad677e884d6c3916056d94661e12fe23c9defe7fd6e854d2" },
                { "ka", "90ca109f17d836bae4c64723dca59043ee11f2f917855c6197981eb680a9fec53c01d44df1773d4de816531609bc03cd63dc392deae0909b25c9fba5b54f9833" },
                { "kab", "b6b4790990714b0538d06483e7ea4b4a1ed05ac876545d64ad1149b66fe6d5a7510b0caa93afe65c8d699a491550f5d824fb324a486b9e05ac5aa8cc32891a0a" },
                { "kk", "969377d9ba824910e36f85ee6e023f8e4c87497a434ab549b87e74ed6a7a8707c71c9569998207323568240fe8ea27debc5a421a361f97ef6eb81efbe51081ca" },
                { "km", "4871546e1d34a07364b7485b5ce43c93ecf78e6be0046559129170c55a9b67031247de1bec817feb30d22d9a829eb63d04d34d0169a20c3c1316f47def591b3f" },
                { "kn", "3e0755117070120a1b99064dcd6fbfb820e026dc040274fbafcee7ac0562b7606bcb31ebd8fcfb2ccafec097ec8892acfc920fbb9c746db1d6feeb3e36eece22" },
                { "ko", "9a1235a46942ec220d7286c819941b60645b8ab4095dc9b93ee956a1341026cbc1ac06931b581dfc372007439de143fc42b62b442951c42fb809eb6296f5e39d" },
                { "lij", "94c504e64e95ca484bcf1bdff252adcdcceda39525d4acbd4e7736b4937079ec5cda4820a862223d1e89d46a192d6f18b8863006d79bab5c0d941168f4fcc25c" },
                { "lt", "06b3da7b7a2e9b1072b1dd580d99941a4918d49a3d2e2712017e7e26489918da891286a5ae89e2da18b545603cbf3ec5ec3c6844b1d4579ca19d4d26d320fc11" },
                { "lv", "16611baed51cf73db2be78f50433ba9aa772ad52e731bcd2074435433e8b975952064ff4967db73da219e1bc18100b9885069d9c10eebd94f86c5db61d781d2d" },
                { "mk", "989f2b5ac7fd7a514af71ed7e8ed5cb5a153f7f70cfad5e3b80a7d6e5273819c1b2399d9c892b1ed5901cc489b75c4ad962a11efd87a3b4aec139dcbaf020923" },
                { "mr", "5eec81ef281c42e4a9ecfe176c4a6d23a5e61c09a4bfadd781b100b9a9df9ea7fb76300771d8924423c7713ac5b8dcb4273fb686c667809fc9e6495143f2b69f" },
                { "ms", "8bddc2bb1bfa472e5b1cba8dab3ba6dd0805243a00c95514dc1fc17eea3c3e5b144fcc63c46f351661251dca1f37f18c121bdf58d5e5cfa545cace6a3bb1fc6e" },
                { "my", "4f904f3c3079f4836eaf8d400e79a828b48d893e395c5f5d502c64f200d4dac160d5b81486fb277e26ca7278ab8cbafb2627553bb933781a38b3b84285cdb894" },
                { "nb-NO", "ab1a25e3b2ce852285b49723e6f6e74eb96610628ed1a0625de0da0d397b0013b6e771db6dea639f67fc7aa9fd3f96400a2085ffe4aed77a9b31906d8ccdc604" },
                { "ne-NP", "fd0c611a8f9258608bfb05c3e590bc31372cd0b84822e7100a5db81856e5a5bc7beb87c78e893b489b05a054c54e4c1c1cc8feaf753ae66b35420697197b3dac" },
                { "nl", "a7400dbe24d2ba84cd095dd107714fe0412e1c78a6077aceca8bc71b63dbdea6237a1abc67cee6c343e9411698a5835a2d120f126b97624b185ae83e8284cda0" },
                { "nn-NO", "cd7e60abeca5c9fad6ccb26cc000826d76d273665a8034de7b57636708937d7dbfdda72fa25f2e09bbb89f6b59a9319360a13cb22a8ad2d365e0c1f3b9bd1c45" },
                { "oc", "6bf8f29324477cbb67ed3ae7e5262ce763a6a3f4ace394bf39b52e505476f096f3622355d697c19ed818252defe0e5a6e1094d7f07aff68df576ccc245588027" },
                { "pa-IN", "7dba0d065d0707dee159927714b2cd28a20042632b0d56ff1b6e1e81179532d8958d60608d0e7e109400c6f1f468388c71761dce07d5dfe370b32265859d7166" },
                { "pl", "86c177d5e0bbffde3c31e51c757f8b7f9d5e2349f5bd7c31e3addbef74466b2ea9d4a69816bcb48a68747c335f6bf4b054a44274c475554f74c313c314e134d1" },
                { "pt-BR", "e8572b3083bc0208b6e5e8212834690a4222b7a7798426364217d988e2ca3d595b621ad9cd59d1d9e1df571b210ca4bd5e8bbc5f94489631f448f1e9d417bab3" },
                { "pt-PT", "539eae87a2a3fa3a0c3aae5e56cab5a101ac05a6914b6dd5a5790cdd9607564dd2af85e7a6fb3c6d817debeef854421d009b2303baf7aaa7899a01f0f83e3be3" },
                { "rm", "f55be9ffadc1ff7329f6a0c02ef445a846ced4bd64ef8cd1b90bfd5513d68a59a36e669c7b14864709faf085561cde0c4079788bfba15acae6aff68b7cb2c481" },
                { "ro", "1e2ec7877defe07798c34522df11f2492ddb1cada4f970a6f6e966460e8d292998ed7b251fdf3f5a3d696e0d432cf94d76992e1e1e2daf326c24d456a8c3cc8d" },
                { "ru", "3adc77b044301572df171edf00397f6a539f171ef3b057c282df02cd300317bad6f4f3fffc4532a2df887adddf940a88c8b547f535677bf590e40b15c1199515" },
                { "si", "0030b92e99cc05a7d1202682d0f747359bc9667f23051a27ae0fba1eddba6a7416b686f047dcb46444c7f525e2f06cf94c362fa5c0ba338d311504085b7a76aa" },
                { "sk", "4a6661128123f8cf03047f943d25feb822c6ab4b4a1b42146c17f36d305c6db4293b0fe9fdc82ceb23c58f2a5f822038b78f913fde515495ae11912edf91e656" },
                { "sl", "3e4c428661f351bfca1db90d9212f366c91dfd6ba371072d2c3a22395cfae99919d04f2c4991a6d5230903203dc0eb719d8fca9b1b662ec5f3f7f5fc1300b99c" },
                { "son", "c3f9eb85b32b7822dafbb0038e6a3ff9db9e7b6d701103b31dd6b08d12f01aebce462dbf85995a29fb7302a3be628e6612579db478194ed5dff79ae588abd67f" },
                { "sq", "58649d97199e97f09b32ddaf777a40d4b1d78698c47ec4144e3255709c6682b15397a41d13f5da55c4a6c8aad86c7de6287229c6b3d35ec545c859af969489e8" },
                { "sr", "8409fb185e422366127624fd8fee2b0356894ddc6345e6da757ea8123894bde38339014a01500df40bed58b636a5a43946d2806be489b6530bcfca453e48888d" },
                { "sv-SE", "1a3c1f6f0673fa2392e8d8f2475659d4842332827537a6c0496be732c028710ccf676848c0c20450deb88c15ca3542e32719aac7bc96ad534ad8f59bb42bce99" },
                { "ta", "43875e9d8624179ca8c9880aa647f5292c60216751440a6ae96b2e88405f9e1acb411d8ad31158884d814b3fbb0da6532e7e204ac7f657c1cac26ddd14cd8ef4" },
                { "te", "8bd2ad37eca0fbe447ad296055dc0162cec6e6ddce50ec3732b3c99423798bc57f93e483d8d6d98f7bd0e59ade6ee9dfb699156bce3915498c32d8327a12eb05" },
                { "th", "08355d9dae7122cbe4d70df092dd48429b6fb69512244a3f9122fc9b9af13195de3e71d6165f6a42b76e2c27b4e98ed6657fdc34335bf4ecda3ffdca4d509520" },
                { "tl", "4481a5285a879773925f82477d3ae6ede8eb4dc250d140b20b86e2ec21cdb49822e2cfb394325b1a37b6d940ad1a252022acb9550b0cfb022a2a63fee5efdf45" },
                { "tr", "e108c19ec72b039c6ff2db2fde4bfb66cfd49f2ed22501044aeaa37e075f2e83602b8a56064889affe7de8d110fe1813954dbaf3fcbe4652dcae874d4ec40db2" },
                { "trs", "9faf81ecfbb5ea02e57917e31e7d09a5efbf8f90bb7a9c7019c4c208ab8774e5e62ea3dc47a995e542b1be471436296bda3b3b230092e90307a88ff65a6ab39c" },
                { "uk", "edc664eadf28a715cfbfae96d335563fbdb516e6f56da70cfabcc8ff1d742af171c6e80dacc46d155a32154b9dd8d23a918115e0083ef8e84c2f5deb9cea2519" },
                { "ur", "82f9e624bc261cb071756619a3efc377b0d9ddc42c7abb6907cb0e075a9033ffde4343938c94fbc79834879fe075ca991bb930e087f691239d6e4170d615ef35" },
                { "uz", "3a2c11697f4ca4520e9370ba4ac4b247db3e4d5e202759a57c6c1cb6632da9d81d4311fc054aab3fc9cb2a8ce34bd782c00baceab34e14657cafee2972acbadb" },
                { "vi", "c54d9848ce0851e852e0b1ccc1df0dd68637a0505525d212159bbfa90d2d14f645cee04f469d266298ee952d9abd09eb6d551f67e76875971afd048098a0618f" },
                { "xh", "1fd3b86c2b6fe134e6b75d83a345fcb24a19e88af84c3eaceb3a896a9c2a0410cf84d332c837796bfc416f18f5bb5a06f29bab4f4b195488b5e6b92ed17a2521" },
                { "zh-CN", "4d3113531400594cca3400dd9b99b0f09b12d67bdc040d014dd74a1d5cd209fcdd83b6064a33bdd64855d39a3981175adb1c3d5e67aa560f7962c361c8507e4e" },
                { "zh-TW", "9bb4a1c82cd31e57b5c6bc8f877cbcbe8ecf292578e2ddb2f4f635ffcbba6c4bcd9744c1cdca8efeb6e1192f04f15487b1fa3d24a32377548b31f4d87d5cb39a" }
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
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
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

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    //look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
