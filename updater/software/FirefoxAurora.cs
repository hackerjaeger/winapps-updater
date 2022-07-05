﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "103.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/103.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "6fb52a557d04bdeff0b7eb545e57fa33c606e957a8e253ed2f7438c323ecae2997114ae0556eb9edc53e423360c9c90fb2df207b3bc0b4af0e683f27a0c6bca9" },
                { "af", "29052ed9d040426e8fc503908dd31e57bbe79eac773cbe34aa7b24b5f8937f72f75fef2f5b48cb14a26873ccbc45f297be4f4e9c9ae4175043c6abc322701697" },
                { "an", "06ba4dbaff529623805186068bc91ab9dda2810e80529cc1e30c98000f07294305493ced423e3b330a94ed900c9b81f18cb251531fd0a8d064d62683b8d0062f" },
                { "ar", "7a3bf432dfce3a8cc0212d53fa3229d23ba7e9bff87cd1cb841957a365867bdd98c48f80db27b9b5ab94a5caa25d9e2c7309d45f8ef19d0d141a271107422405" },
                { "ast", "0b5d688a2fed8bdeb7913c7f2cf3aee859f5b90f843ddd9b3d294b757c6fda44aba17472916737938fbb1305beedf809f2780e54e8a9ce91e42065befa5e13eb" },
                { "az", "3f3e3c2d905df300808fa9c3c0df509fbe07536344e800cd039c51d2741d4e6d95cbfdcedb8e298516ec9ed6d45ba869d7671aa528eb8461ca5d62da72f0bcf4" },
                { "be", "dfe9229c34219e8d8857528e68be2c569390f9e9ad4766b92ee004476ae4073822fa215fb9ccf23bcfdb96c2d0fac638cfc56bb67047ed8adc29a6a3bbd2d850" },
                { "bg", "e471d5797bee950ac88d8ec4a21d6afb02e5b3225eff96d9349b8309b9cd0d1f58ec00ecba57a89d3ecdd3f6f6bfec3e073badfd0ca27091bae666dd292e5411" },
                { "bn", "cef0b2bd2fa18d3722a83ea39ab27b4e5f8b1bca6b3ccb3f657b4c1c17354848643bbc1f759d167fcadd6a7846116f8ff7f9bd4585ebbd11a5ef2dd1d8c569c4" },
                { "br", "d5feebbd3686946aefb73657ee8274215ca47bca4a4c14031a0d11bab2b1e8fe35ab10691f534fe07038436ccc92aad9bcf97bde65117cfa38914639e2a107dd" },
                { "bs", "2c8be71d25196f3eb58fc24d233e1b59db9d3f9848a04d86b79512a3b04e1a7c81566675635f0e9fd0773f5e300a389637b5c7b0b2e6b80b7236231702b52c5b" },
                { "ca", "44bd7316d9859b148525549af34dfa15c4515389e8104bde45fdc6ae79dbc852f646d39532fdac4544fd34bae64fe580fb1b8f960e52ed7118fb5648ddb837a3" },
                { "cak", "ee6b22a06410a7cd846c177c26c3dcc520a40112bb4a4465f9aa9bceb2d35d6755e2c351bd31bfd818062538b152b07f626b7d1f60e6dcaee00166ee4eb31d80" },
                { "cs", "3aba138472adebd712e11fe74aab7b75db0845b17e9c84153e21f99e6f4903021425b6cc25caab1269f01c01b47229cc1597855473d8b1e57db8b527c7fed8e2" },
                { "cy", "eaf9db0c9ea6a79e04e46c2301701f6b3125dbf0bb0ce43b9d49ec4d241421809dc709e8bf87faf28fc9216031be770712d6875833f837aa6bc07052110819ef" },
                { "da", "5f00e7c94ebae4c7fd32ef94070dd9ece8ff7a7b7e42ba9d01965751826b7e38dec08d9835998d3eac3c494fc9e975de04d4f28ce5002676712a68fde735128f" },
                { "de", "5d5d405b6be3129f06cf5c842d9109adbac3f2ed5be9aa3aea6c30a49857a1bdd24355e5318f97760c53d825fcb8bc4b79bb6fc866553bf66cdc90bcda0b701e" },
                { "dsb", "773a7f8bccf675c2c1258996caa055a227f9c66599612ed34792b057651b322b87cf19c8b484e6d3804cc8dc5d8c98d0f424f1a628aa93dd31a3ead78b13d0d8" },
                { "el", "77e23283033222e87a0d2fd436aa4049be27d414ae06a3ec7c8006d3d3755bd99db8be8dc2530cbbcadc18927f5f3af62c871ce51e0142d1d6e2be8fe3ca49f2" },
                { "en-CA", "702100cfd4ca45c884adc38a16b46290b50ee0a14f0588b4cd52e4a21d66391dfb87051482c1ade5b8606b8d378c7d57845bf43a3fa5496e08ca6b87f7a8c89a" },
                { "en-GB", "4d89e429d1fc606aeb8b3a78231ebd7c9ef8fd28e59142ef8d0ff780c62eabfc5926c9baee17c83be8a90f94f92f1250fede7007268cf04380ab89d1120492a2" },
                { "en-US", "c273ed58010337831bc830caf1173390960242634589a9cf239b1ecf5676449d304dde6d67c6c3402f09e961829df4ea9941482f28b15c3955c1c111242b29c4" },
                { "eo", "eb0f7153b88599e824e6aa67a389629e483f1f2ce7a6b92e59a1f6aff4170cdda3d0114fcc4610ae0252d409339cfe5630d3d5e393bce18dd9694532eea02c24" },
                { "es-AR", "61a68fc8a98559236c5a885b7700602be34dd3f19821fa7a9467402759bed37589215a8c080fafba9ff92e1ba4cb8313aabf11867d892bed7e6f564dcc87e2de" },
                { "es-CL", "6f01e7d5d8cff869207a057989a9c45c07eab5e4f5394732b7463f21441e42bf3f30fb5e7f4f7f1483d9abba5f5d6606dce0f112f114d26e5eeb0f6ecaf48ff2" },
                { "es-ES", "1b514635bad99fdbbab4b5260e0a1a9f038ed33ce249bcf02d8baf65519897adc31b0c96e66a503ac3d3fbfd12e1805ab5feba7dbc5e28e411953b429dbd9198" },
                { "es-MX", "75d7c9b337da62280c1265468e1a127c690deabc3a6c5b1d4a98d607f87f5057f304f81eee04ad8fd5e031a565e289af0ad62daf18092c45aec9720fe9e9e5d8" },
                { "et", "67e3abbda11ba65afdcd7afd236eec2106d09df1175cc3c59834dab58ce326c2fec5e1dfa892d7e836ec8d215dbf7a6cfd39c406b997240456c45e98c6ec1e01" },
                { "eu", "c6f256441e8ef47a709b3cc53cf764e47d4b669232162d73b55659481584766c83c61b852426b061927152428c934ee4c6ecdcf49659d7b10dbf817116d95f35" },
                { "fa", "1d5cf517917d277b93eb586148f5be3b5c406de78f71b02d582aa09bbb5123617f12b06af4cfdab2988e492e448ed1315f40fde7d4229de5b26da50a1d6103bb" },
                { "ff", "3968b00389f11114522301e83ed08244fb723c79821fc0f93c449e69ec91b4a7231f31978596bddc26da301d50afcea2e378968d13e53a624e7d90aa8e7daed7" },
                { "fi", "428332a1ead7f8ac7fe738027b34665f6ac044721d000142e0ff861f404518e4b3263e40c80eac5de26defbed62ff613c2eeecf2b595593613d6c3d1cb55969c" },
                { "fr", "f9ab6e3cf24548bd1469d4c7c7724aaddf92f217873367ea37d4b0e93ba32f622182442c75ce41634c7572ba1e18a34d531876acd826b22d0ec7b414c6264904" },
                { "fy-NL", "e476c2813e63aa1e1b2f965a086eede0283c6c8b7d89e57f353a957578bdab9d2284dbea847c583c845f2d5f0cf46aec248be2fcfd30d9bd44619c00c983ab5f" },
                { "ga-IE", "830bd25a122eb82544abebd3bd04da7383bed4a42b9eb4fec55c84ce93d7b3804d53ea3e2d8cf44dd5439775b709a419022d788fac7b802b15338edd91b96f9f" },
                { "gd", "c92639d07840b4c380dc549a4a5b0d94f9f3106537e73a1850fdd013491f66ed06b270948e27c4042af45160354a0b270e67d2c83ddf54b5a401e5eaff0cd9c6" },
                { "gl", "5687e9276d3a0f61282022408f4edc134488cc9e9331133e9f6b933a47c9f0778fbb56b9d6ee5a70cb92ac7765dacb3586035c2cd638aa5cac72cc39f49bf07b" },
                { "gn", "01108479d935bf0cdb766ba9909d823e4661cf8b5241da060d60eb5da396cd79857f6cf4366785e8677062f57953fc323713696a0b301af52834172ea0252d99" },
                { "gu-IN", "dfa115e6240becc91012cee90be6f81d116884f42303dfcff65c1468e540e2a56bae4124f034322a44a78eb22f2f9db260a182f5fe91edc9c77451bd6313b903" },
                { "he", "73eea3e631a45b5c63f8405e4f1a12aa015b32af2d14321d932a9e585228b9f56a42e2ad7d1b05a2fedf689ddd4ea2e4e82d90467c358301a7ce6a6c5391ed86" },
                { "hi-IN", "1ee3db2337eeb20d2f78aef62d20b404f840001475cd9602f2a4a55cec8e2814e3fec4b6b878985b98ff490e8a1fb9de149f3569e7f147aa675b1eed4f093e15" },
                { "hr", "63884da1e9fc9d902f5cf761ca6383d7078bd75cce05ed76e348d336a38ad9b23cf7967e659a328559edfe84ceeef76a7b06e90d9c426cbcf280dbd6a7e81c44" },
                { "hsb", "38a34d218ebbde3b833fbadbb6578a5a18c9b68e6c87abfaaed011e72881c8be1c64557fb3d7778a1782f3de03b448534a1a6c5429a4327cd8c6d32227784ebd" },
                { "hu", "2ded064a728c0fc12ec78f8c318dae9474c1b432e67c5faa67b25da4823d162527c70b886f86b87d1cd7209c06987b6180884dfafd5c9a33ed863b4b70aa7d85" },
                { "hy-AM", "66e73c72577b01dfd90e6474393760a18b319bfa81f98ae5a048e2dc085af363e440ee024134be723dda0bd40775b82a03aad8dcb5c11eddd3e9f46798b9055d" },
                { "ia", "cbf4cb1c172ec869d655fa64dc6a9eb3d95813905d7aec73cde245e44a6c2dd0209083fa0b9f8aeb5896f627aff72eeee2adc69f1b81c5a6bb875e6ed5314370" },
                { "id", "b23ccab1323767a773bd63c80f3e55d011ee423876bbcea50c5085b2bd93056b8502fc970e70c7fa89ba8eef9fabb94051c4b13d3b1f4814e1d8369a9a6625d2" },
                { "is", "8d17909a33557b5f3b8c288976f6027679625b1c987b7b5f38e0a5fe16a7d24bed21e6408784e8ebdf1b67dc753954ccb102c05c0a958559d821b70d97e066dc" },
                { "it", "6a3e48e0685dd656bfb58c911d34f06147ee76ac1cddd7b7d067b97e27ea7cc674f2996bdb6dc3198b6df87323aea3e9585b0b1bee29ae117c49f72b24539802" },
                { "ja", "ba4feaa2889aecf23615269edff697036e3b4020b6240d5e42d9ac97263dad6dfbe026b80e000d3ae6d08af4f571713fc8803826fc55a4138e2e6e4e76cf4a47" },
                { "ka", "eee53f0658935f5dd0d7d8921567b4798c5f53cd4e5a60d6a59568630cadba303f86c3bdcc5398b0d4bd5ee1afbc785ca29cd4165c63f401a77f8323bf77023d" },
                { "kab", "d545ae491a17eb14f9a0305b884ab80f1a0cc4efa2cfa2364ce7c117265968a5e1655943b9ba56964f2cb9eef8e08df49b6197d849dda962122f0e140ed0a310" },
                { "kk", "17de3771dfc70caa7831b9127bc59212e7b7eb092cad75c5a40176ae3335d745a3d6985761c1c0f63c0176e9699a83bf178cb02c215d0a012642870fed5c85cc" },
                { "km", "9a7a8c0ef6035776e3e7883c10ce5b84e36070c82e4f89c0b80a336e11c83a6d1989b4b3ac68ea36419c0f4055f47af5804d0867b527ea9e6ce2be1d50d6c38f" },
                { "kn", "549ddf79f70a4802607e0a6c52ab2ea59ec04ce8b135164065adffac0ef1efacf5fbc9b2d450cb0862d88915340bb9f2d955feb1887a9897b1e9500bffb03e09" },
                { "ko", "48a76c7339097ca2065e1ed0bfa9fb17bfd7f3f746cc6c05f07adc4a508806bb0817573e193f7a93b9179953335be0b7a7037b36062b75a85007694bb51597c3" },
                { "lij", "48ba747bbea117c04530263c122c97e5dc2d3a60c131fdabd77e5e55b9b9af25cd7af8610a18f96c5619be62f28556b58f9386ba2ca05f17ef72a4e59bdb9f5c" },
                { "lt", "7c0983c87376f947cd20f69e34e44b4bbae9a339d4c83242545a5ae58faa0c10e6c54d094bf5946a6e045c3ab0f63e26d3f56da97d7663c91f4aeb176c019493" },
                { "lv", "9b8c3a11629ec934b113c7313ef369081d3183f0fa91560974f3fab3cc3bdbedd1c3a29a76d54fea5ddf1011756db15396bf1d6d1021322630156e521280cf3b" },
                { "mk", "271690722a653272aa48b175da11ed1b4be874b4b7f6c668ee7aefced36c50b46b47544b6d0eda0a9f84bdf14629d452bff4f8d235febe7832e05157ee11ed67" },
                { "mr", "4b9977bcdc76d0da732546ad4dd84881dcd19586c729c46203946832bf18e991437390b185198069bf63a2f83a8ab787e818c031d8598ef465cc0e1fe2df96e3" },
                { "ms", "c29ce6bff921b7b819fcf8c2b089c684db676055e4b54f79cde3593d6260fd1147686b0d9dced8d9274863720c6ba21bde3886b64c9f7b5295a1580b16c3915a" },
                { "my", "2adaae458ff5accfb03ab87f4a7e495935d56e5051696fe1e55c282def09301e76e923feee47cf8a0e83a7fad106ef7aaedde91b013cffc4d1dbeccf0338a73a" },
                { "nb-NO", "7a90ca07ca43819b741aa2418025e6006d2ffb5cd970bb3c71dd539f9da7eb12c15c2b5767c8ac290ebd024f253db6762a4a5feccd5b09f78e88deece44d606a" },
                { "ne-NP", "2cd3646211af5a211921767dd60c6e6bb17c43f69eab5a2f6ba4299add106cf6ddb2a400939a88db3fccb21d6ea6fb3a9f556ccbb7faac348ff256d1342eceb3" },
                { "nl", "ce4a02c7e99dfc693b8e235155cdbb1c1b6855dd47b4f6dd3018f6eec15d1795c3d5206ab5d76f6d761958c40fd8fcb044b57c0b7b2eee61f598c8b12aa4589e" },
                { "nn-NO", "524803868cb64c83b2bbdd79e6e297f861b0bec00683c984225673e26c656171995b4a0c891e6b0523b753fcdd193a477a6fe819e5473de46b575c6d1bb48787" },
                { "oc", "bbe3c84cb8d54c697cacd101f28449d6bee8e71e4fce745d5d8fb2f8c935fb9d7e30c0aa4ac27db41635d5270f4535ce760417853f4a21d3f9548710fa23201f" },
                { "pa-IN", "df002d7271c7f3f1a1d0d2bfa3b3b868e7b7a25afde0b92ddacf199229dbc5d7e22621359fb74fd69e3d2f3478afa31056c0912b16dbbd60ad0fab6ec8fd61b4" },
                { "pl", "68d5eeb28f143177c7d5f2ffdf50a7781b9d32e0dc0146ab1e1e2f81e24f114b5813740c3fc57ba33a219f391d1066b82907b9692875e3b97be1c345aaa00007" },
                { "pt-BR", "5c2d076ac5a0aa93a473bb2c12e05f68651d999e2be5993f11cfec2c1b03af0da0b9cb1d15336d3c9c10f067f92fcdd8392c0e344d78c4519d39097a116a2eb5" },
                { "pt-PT", "c28facce9552d038272cf645d9f6386cd6081f93db5b4404bc596cbba32ad83bf8ec5fcb99b2a039c9130c54a64a4a97aa8479f0b5b61a573868f0617c689c2e" },
                { "rm", "b7a5376973248812abbaa6ab26bec1e7a17091beca724aa3079f831d4052fb56736024c380ac7e81a287df22e574e0601aaf0c190d45fc8f2c17b8e7bbb5b27f" },
                { "ro", "b3dcbacf1719274511fd2443ecc95e02a2bb21ac11439960f385554085bbfad3e289772714c60f7cbad44579d89e421f556ef16805509e353167cf2b7b0e0137" },
                { "ru", "86ebdae976dc85c39e3bce97c04f86ba683ffac193b004029d62761fc06e30d56b589a03286c4128b0c337465353e686ec49f9c22117d52e93e44400ec011807" },
                { "sco", "88a135648c90938593ad3572adfec2f6192e8dda1d563fa7cfbd3754f952a845c8aaf5598686ef42266bef0322808e083e55d8a4f514e20e565cb33d6c4671cb" },
                { "si", "1b713c32ac5ee42ac9fc83cc996c8b1a3d2f404f5b153779a7230f556756e51bd3a0129d4116a1f7557cbb4194572e6fa02c85674f1e21ff035ee85e6d3b1707" },
                { "sk", "ae8e5a806eae59c6ca009c4f49ddad379d476aa25d34679073dfc3461e145c30fb9d5787f8dbe552a5be856d4aedcdebca15bd75078957e9b2b4e52c8bb96e11" },
                { "sl", "318b1da12fcc4750544c7aa89cd9682aa5d048ddef776e6a172bb50a26f7ccfe4da72aa805379ca7772af505757221057fa36077ed6cc2d31c3da2b4cc60229f" },
                { "son", "95026ae143e9c34f418fc7bce04be5d6815cf56b79a6ea50242031507bfcc07e268ea88307afb3424b07c5def676ea22b72364b5c5cd8e637228f665838cf7ff" },
                { "sq", "b0b9f5cf89236b535409e3fe4feb1695b91a7125eb5b90580f6e6970661b3d7657d2f563f3d5a7f657f58b7aa1f84bdd7dbe7cf137a01b5ce2257bd2d185cdbe" },
                { "sr", "ed53bada981c431d9426f81909818699c9a4a6618842cb4129c51a342dd71f1dc939f4e9a5fb81b22ba9c04fa9268e8b28a25125d8ffb36c4b671e7fc2ddb9d7" },
                { "sv-SE", "46328344fca8e96dcf6c442e4951f26934f6131b8a9af3e3e63e1fc5f63f12a10c0362eead5654be29b975053e234cd3a74d97cca43deadbc2bcaf9ac3da0f8e" },
                { "szl", "77d631d9e8a0e1b9b2615116ad50f000a46609991b04dff39e1b71d45bdb91bba3863e897b94ab229e0e8289980195169066053b314a702241fed70c3a02623c" },
                { "ta", "fb105fdd88f32226210e67f16928ed352ae9a3fe1e6f3cfb03c740e3dc0f04666d9d7c8a5c1b1d0bb651cd9c0ac27b3579f1d3b20ed0576facf7ec317c2513cd" },
                { "te", "681fa12c0348327ce4184dd5495d189596db25bd07e7574644f9f9ce442183f085a73138bc546e38b078e34d5a7f0314508684a2ec91556d5384e0eaeb80e862" },
                { "th", "d864ee78e3934418296debfb0cf6e30057c69a0b318e15344bbb573a8bc00226e4544d88abdb31c05d0dea38efeb2b35048eca647f9cb82ab03b810814fac5d5" },
                { "tl", "5167da3126367fb74c1fc65d0f6c99b85125f2412010b03b77f1cbcc6218c223e53850ab87d8f3ab9d37e7f6b3d0bb88c6bd4ab072813cfca7d36e0cc7e4199f" },
                { "tr", "1e8c40e7f90d9f017ecb345440fa078c8e6dce9a1b0cfe7bce41b1e61dc41c73b7010c6c1bcf32f2fb9a2810ed7db5adac9cd42864a3127629467f3da57b5565" },
                { "trs", "d7650988fbaa767a00e7fc10e863464844f30ba72b78c3ca173466d422429c06f14dfbf4bf37e22fd9e987bf90104230794d3c1a8fbf651eae1290f7300c8005" },
                { "uk", "e59fea757bd84b1e07ecace0471d0d9c7483f00ef464766bf7b4fc05db5aef799d13bbef60cd1ce848bb6353dbb245c82b1457c1e6753750900ba617d4edc955" },
                { "ur", "f71c514c857160004585ba6b1e6f273027493f5ac51e527eecc132d0b7ce8bd2ad0d9593fff333ab7bc3478d0d6a2c4a0274a7ae821920e10b10200ed423ab32" },
                { "uz", "2bcbc81c368ab01d151929164de29a91d3279e7c6c35007a7493ac1eac8bf619c672615a251a51d06f61399eee6ce17912479878ead20959377018611b2456fc" },
                { "vi", "3d0ec96558578a938e6933f1596206a3031ef0fafdbdc4805ece299338f0fbbd24d900e7ba2226bf494f3ebbd96f3c90af26dcfeef7076416bf65f51c928a7e2" },
                { "xh", "abb011e9ada5f48e7e16a90c8b65f50255961f8f190c21b564c3847f0685343020203c84a1d035f4ef934dd5350f5ae0b6f02ab56fea12c34bf365da648345f3" },
                { "zh-CN", "8c5390b924c8d773f2b6c192c59617968a1a66aa18098b2a3df83ff1b52781ba66f4a474dc6922d6c8bc458ff6cdc43884c7aea3f643a9efb9e1a1e7fa15be4a" },
                { "zh-TW", "fc546d96ce31bae16ccc826921ed57ed9e60cd22d0085b02bd68a9205d15b402235fda11f47b050e7065cdf1ea25c7db7c1f904d24e720a18666b0b3cc410d29" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/103.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "e6b0379efdf1fa6cf958495fa6440b02196e59e8c1fe80bfeb5057e697c2d1a06c0ca037f342456b2160688b00cf36eb309d9b98eb5eae7d8425ac864d018763" },
                { "af", "4c57d313cb2a5296c77ed5a2b2ed6e3c7182d20fceb31de9b38e5450b0b5b250b9d6575326b8176eb775f6ea8b734978397bbc912f365d210373144c3fecfd91" },
                { "an", "d218d5b4f985857fb1d323146b3e65a0846757f56ec5074142125f2a14342b4e810bb994beba51c5155f56b7689825024d18ccb61b03e5b70e7115602852a3ab" },
                { "ar", "f21c23df82840bfb70150df5d34361eb031d48096283ed2d584141b7d03667d8d2f90ac4569bd587a1cecd2d28931a400a2ab032689fdd38173a45fa8db67440" },
                { "ast", "f966e5a3cec7ef769b149a5574840ba642713a6f209484ee04598e0ff38fe039251837edcb84f34637baa30f9c4084a12fdb20b7d313dc862e2eb0fc95cceb4d" },
                { "az", "a663224eec830eaf52c9335a3f008cb0ccdf4a26530af259d56ec9f7764ea140180d5e4ad006105a363ba0db290533e0f23c8a95fefa31478b77b3b9c74886d1" },
                { "be", "d777e00e7462d9cb8de4f15dedf9756250ab79f080e3c1b36e5b01c41b5fa6e9d43afed45ae417b48a31107505324933f7e948fe0874dca2c38c8910950a8684" },
                { "bg", "97c4636bb26f2a4e2d6fb6b94687b78d08a67ee1028b03596183c289df0f7a97756ba2636216837c43cbba669a750fbc5a237a1a631571b4a35479b801616cee" },
                { "bn", "e369182f69406a48a22661303741a3b08950c41c1dc3d6709fb3f0e8d525ae7ae7e754d716cd19b3dfab9d0f3e7a1191c779154e94ad095bb2e8623e4177ee8e" },
                { "br", "5f36fe8623c7f90f03a224f03e227448de0e2f9cb94086b24384a054b4d57ebbfb34807b36826fc6d449ed073f1c36a0fb210d8601d15aaf7dc6b32b5bf21799" },
                { "bs", "c785e539d97cfe1af9e3d23212e9df990d918fdffec249e720d68880e274918f41719c5e84b3a0d510cf9860acc73e03f19421c9811a422eba45ac456a98c750" },
                { "ca", "81ea4c554c4b495e694c61a11c841d02b7689bf927db16918cfef782d0a0de590d83a170031bb842d32867d6285913d4dfb3ba0deb2f04fc0297649b2c00d7fd" },
                { "cak", "0298e8d3de0732cd0314ec1ebc55937c06c0097fd1119988d0ac960e827403c2f7629eccfbc3e9c27d34e093c003e850de33846c5f22307074bee70199f9fc36" },
                { "cs", "65d777bcb17c8b60db596fd14cce420af6f7042cb24476e143c3ef92aa20b84d6df2405ccc0dde9c1479ae7878e7a34a31f2f38ab6c84bc24230ff1fdf7754e4" },
                { "cy", "afd0f48b415d5441e640ec23b9c6e95b384e4d4144ba86692b4d39eb89d734e70b0c03bbd84df165d927008a343f7645e32a3386c44f4ca6b0159a8554d05db6" },
                { "da", "04b80ed933f10ccbf75ab2cb17c171ffaf3115e78a9d353dbe779fcd7ec0e8a7483fcef76bc1b9d37566f6bffe8c821d7db979edb0e59bb8a5790d1f8828d1f1" },
                { "de", "d2aa7481380841b7d024dfd1c0eba7a384e45ea03e99e1e50e6fd4de7953fa1bab409c7f2638def781e7d6de7500eef0dd87ab1490b60a0024c2c6b7dc657e5a" },
                { "dsb", "32b8af6337a3845dccf7d1e905a8725e34abad6c85eb1257109002bc3c44b3ee88486493685476930d5686c20079214f6989492375d4cef58f5bc90f958ced82" },
                { "el", "f6e5dd283c23335f3235f73ba8db349ebfaae5befce013fb93cbdc85f8d763ff9f56906ecc124c396a497f7167ee2c3ce7dd1bfea5c9f149cd62739d908170d0" },
                { "en-CA", "1ff60f07fb643d240e35976b45f39e519d16905d9766528f5dc3a2438700957feebbc8afe9a3df0d763923c3bb4aa2e1c14cc2c6aa4099d9e18d7c4221849a22" },
                { "en-GB", "d2d3ba645a77706f0b22cca00398138903814f41abec848ff88dea90b6c883e896cca7a9740257888f08a5534e10992d3c3059fb695ec85d4500f05d9b431d61" },
                { "en-US", "e0fab47d0ac26214a49a19bc024cddc6e8d37f42699edba209c4cad304fd3c7ce64a93c4cd8bdb7d51e14d052bbb9612f4fa4f1587a35e910f175f2b63fafe98" },
                { "eo", "db2b12f65c982149b3b4392ef8b709af97289204c39c2d4a2c9fcf938491527f5dee29ea363818a48bcc8cd0927c13ca8283a3cdeb4bf62a390c3feffa25f37b" },
                { "es-AR", "355de05eb62f394eed5080ca204a0895787497f80af92dee197d0f531162f1e9d367fd33ba0ef87359d961e0aac83e07d2343153b1da99119dc51a739e9cf90c" },
                { "es-CL", "a6d2dda5a7806af773c8c8d21eeb957b5e5e5b01444e490e96bceb6abcc7c0402c3f1174056b9d67ea1ffef58f7bb0dbfe3cdd9fe148e8fcd46189023dddc605" },
                { "es-ES", "735f2e4e0ebc77b037cd17ba1c01d8141a0ae4a67e6b90c8acb5288a617b5b9ca2b7e268e2867f68ce0765e0a092689bfa35b6c0980e965db6c2ae4ea88ecb80" },
                { "es-MX", "772711caa63963ac7adb5035287290405d7beaa770ec629fcfed82594fb1b80cfc1142d8ab2cfd41d3e71a059ba2a79769726c5128b116647f673e6092f4b1bd" },
                { "et", "816c445ac8ff718f1d39b1a5ff35d7bb8808e156816ae91e97d4da40c066b83fb8556838765a57217457ffcd4e50feeaa8d6d918dac588ad6d29553dea30e92a" },
                { "eu", "d28aede20b33d367fbc59015ab85ac02d00f6e7b2c27521cfd928b4445fdc623a5aacaf9c143aa8608ff6d745e9937cc1e07fd082af7a538ea9ef454891f5320" },
                { "fa", "a554ec026fc8286890be8b71f988f9ac611138834d59f05403f1cb25488d87c810483acee23243d7f7781aa3460e32944634a04374b3f0cf0b1c317eee0513ba" },
                { "ff", "1fa254414934bafaafbdf1388166bb239516de4fb4165fe9644896652e9b81946df3036f5f34c0bf1a570ab0473f871e7b77582d1b0076e406ec8192a58de471" },
                { "fi", "22efe77de348772477321d6a57bf6f87128f184e83674f2c008fc7c88e06156e830bce4552e393c760dcbd503ca47571cb818eff4ef5a745e1edcdf0bbe2b132" },
                { "fr", "4fd5da639957b85c10acecbf399168d633520fb591c6a3e680374080b74720374fd8963fc8e0144506c08753d86e6fb1c36d26fcd1c63a99bf15da5b98ed7e76" },
                { "fy-NL", "54e95d5624bfdbab8aec8700d5ddd94fa21956a9503ad5e24f473023f1c1f3be72f62c83f01c281ad5280e3bee44823abe5a840460510886c93a6bd51873c3da" },
                { "ga-IE", "5ff3dcc4a51520b8f79a562c5fccf4a367e4a17eda636c739849755f1acfab4c248079bb0afe000d16e2e6ce0cef0aca0d03df635242e4dc40db7209500c242a" },
                { "gd", "38151a47bcb62daf22afdc556a60e3578bcb0e0075e3cf6fe4d157c5dad7de221654e39994ac28b210739288768121e1f9bd3374069b63224da4948ac9d1074c" },
                { "gl", "318f121daba76956e35acd2b3fb7c8b90427ab046ab0b8c708c6c25e6189b5dec884785e9ed7b43a62871f42019818fc3a4db21fcead006b890803757eb3a303" },
                { "gn", "7f9ebb7145ea7b91fc242dcbca3d972116289135264cd3fe97559decbde2a9d2eb2379f0883400c993bb0750fd743f5673b9079de770e64da06ffa91ced921f0" },
                { "gu-IN", "3619e77ca46514c544cd47586b79eb374e92be9c9aab6c2df4bfb47546cbd30080321791a96fd692ead054eebba8a740878ac299c70d4470f998a974ed7c762d" },
                { "he", "b0a234268bf151f67c2e0880fb040d60cb516516483c16ce832f80d9b0ff51981aaaa7a62bce17bf42e2d593de594895804fb0ca8d8390b37bd05255246bb527" },
                { "hi-IN", "287dd11a9965e1946b38d08302bdd8dbb6790c556e851dbc46b0d0a35f36ea2031b34ad2dbd44046835ca65d08c3cd365fe447114a72fe51c703df94354d2d34" },
                { "hr", "94ad1362b405f6b1fc02cf7a1323dc3d14164ca5e655598061a7a2e6560b03cd0795f6c4edca5874daf5fd44c325ed0fba02e4bd875a8d01beaac336cf3eb985" },
                { "hsb", "4795d1b0ff20f4e552b2caf3ee71365ac73ba111d117197fc807293d1ac8c5fc008bb1d90c43504dc0d5d02dc5957402c34fc21f1ab875d854b58f93ff260d23" },
                { "hu", "2b8fcda4d6fd0c843812b16e984ff4914b3519e0052aad9472dfaf6f3d1451b635945aab979e4b511ed89d40781e0941847e4d8d5027a77bcb118783b7889537" },
                { "hy-AM", "8e5594c8db90c6cad66c4c1b89108b10fd1aab0b0ec63305c36ef91da6b954902a3c0ac33f40518b3cc420710a6c465ab978a26e0ab3c5ce7230e6631beebbec" },
                { "ia", "01cc6f2400290dd57b9efec2741ef959688fb79dec579e1de7fd768b705e6f5277242d54dac85160a81cea8e4e28de4c473e3978f13269d7264d62bbea406054" },
                { "id", "f5197865fc52cb72524e3557ab28fafea23d7b3392377fb23711beed4239aff89a9439e843dfcb2a031637c7acd3a32b09fc400e0d854acb788fd7f006731f40" },
                { "is", "468a6d826efb5de93553fbe3f12fcb834004ef972ed9fac32f7e90e746554cb28f4bb53e2ba667b76e63dde0932fe9ac65906745e4d48f7afa0b5a37d523d985" },
                { "it", "c8049191b857bb454413a9f3200ce26490c46cc17d5c6ef16b791391defbaa7c1ad0d08702991c5ba3dfb278d2f545a093116b3c302080a449e9551f80ce1303" },
                { "ja", "c6e669beb31f7242f5f3105122b8da4f72b391d68722024d37831670e83b64e2ee5b642af89d284ce5d54e8b282de5e1a72a10a20acd130fb544bdc664043909" },
                { "ka", "327fcae1cae82cbb601a5f7ac7178931df6bc2df5bee237ee6c41fe445c543549fff386b348bf23edd1a0a2465461e84264fc1a50689d91da232d73f8a87e8f1" },
                { "kab", "278bcef745e704bb83b94563743e32c0201c5280dc8ebcabe7ffbeb45542ce5d1716910cabc0355508ecfa31d4b642375a9d63257aa46b4b3c916a7c5de1d03b" },
                { "kk", "c31d5edcabb175ec7306956005f014eb49b12075963e3fa37b2ee7145a3f48b30644a700498b1800739a3d384991121433c07906674610926049488d0820d061" },
                { "km", "2b0744c41410c0466f387f8b91e673804300310cb823a721a468f7501e98178df19ae004367f17f6e93b962f8aef3f83137f13a0b653c726921ab2686dc119a0" },
                { "kn", "a4eaa26dca08ddcf2e70ce4d0e29713635f2a0b19a1ed5f81f4302d6cc6df1b0e755ae0b95f098476bc1eb4857fc59203d805f10394222f68400f7beb7ab0b2d" },
                { "ko", "4f096c4380b2c0434a6956cc06764c2e2600fa4b40074456b6349e6a16428e41126773ed650ac3294ad117069846043ccabefd0a88dc3b9ca719048896902b90" },
                { "lij", "9e3dc8f2bac63f5f44f6ab47036efb96b81813b099cb6791552cf3760eca1d55b7efdfecd669c404f16568d9c19687876a61e6fb07429da6ca70e0cdd7a64ba4" },
                { "lt", "64e9d03168adb038c5836dc73bc92b0b2f57d579309705c71dfec603f9d23ffbd5a40705a5ee94944206ef6acee605bffb372f27879fa4054a2f4180c938732f" },
                { "lv", "ff5db48f683a6f459383010ed3d7f5ce25ad58d782664db73c638a820de81a57b5ae9f780c9875b7ba259088e779f6b9e6b015c3f456969b787ca70b6a7c465a" },
                { "mk", "cd851ce4a6f1ae206c7a4efc69148eaf5e828929994f682611446955494fa494adc757239ae8081fb73ad8bf2006244774602376992f10ddb16357c51bab60d0" },
                { "mr", "5b1462c0e65cb84aba9f702c5507e0f2a1707bad224339d77925d30a4e2f196693ff06819500f0c500dfcea35f3be3b4c894a3aa40ba48daaaeffd9ba2c9439e" },
                { "ms", "d59d32c45da1cca8565b3092365983971cc4a6a4279d9f33a00e7b1da212ef0a537f014151d836d2f6069ca45fbc9120a68f1601d04725c200104463f5ad568b" },
                { "my", "b2e41be0ad26155f1ebb5c99c4fdbea1e4f2bd321deaa2d7b33cbd0a04f476974268893db67e823e379a2e768bfee0d275264807c0f782cf5c34cbc1715fefed" },
                { "nb-NO", "60f1336cef10ecdd37d1a402e957df9064c0dc9facacda7e87646017d03061ad916e8a1443e45974245e77b887b08d47c0cc34bf1a6217b35b7aeb96de163680" },
                { "ne-NP", "c7c82b4cbb3e9d4caedb3527d7ecd64c58416ecec80f1f757713f9e297a87df75ccd52309b18ceb2657ff0914a5363b7439816cb48b81be9390a7a5ef3441da2" },
                { "nl", "f63ef7329aefc610a0f8b754ca7fae905957823b1caec5af008f9ffcf0bb2f0e8475ed0a72aa68778e36b527a04036503b4e4fc79d53d76b5fc481bfe5323865" },
                { "nn-NO", "5619e64495503a3bed245382a4381a588291fee8d2920f9022bd7f3ba5a7a3f4fd4c5f6d56ef4fbab3d62323b69be561969b35356dafd2a148c5bee219175458" },
                { "oc", "81722be36a7c055d03a7e33db2c211a45d658b025ebd10089eebc41e376767ff2ce1600408461071f275f307939106eaca015b0eb608693f523e8aa261a723fd" },
                { "pa-IN", "7952d8e0f9690be431f7d0943dedf997205ca13811d2b5359f1ad4bd17af34ceaeabd60f9c3759bba0313372f60d335bf8d56a30bd20117a3a9a43ce54ee24f5" },
                { "pl", "82e9c3c61310b47daa8b83cf35d62e95bdb819398bfc92ec87a3ae307e35bdadbe7edd41c145b1af517bca3cfc319c747d09015737478ad161dc0b7e37084c73" },
                { "pt-BR", "b9c02100c6b7d811eb71751363236bc4a09d776faf10cc3c0ec3e6552fc30a307e89fd84c2762a956c5432c894f0244b0f82f691b1d76c14ec69492a483dd658" },
                { "pt-PT", "414d87c0819aac1e9f67f454ed79419e3cf089d812e973663dc6eb9dc15b2a2fe8ff0e308ad08da6b85f9c6f9b0f41b5d7ab8017ac6d0948fc6d0474218ea15e" },
                { "rm", "24d7af7d846c626921283f8c3c020ccd7bc4b069767a9fcb673bd492e279f8c655ada583db7ee6e5cd5a1dacc550db154f44828b31b77ab330d69f819f5db072" },
                { "ro", "1a3badf1d80fde9d7621309b88a6324d0beaeb2ff89442e2913e544957fcc610947ed134bdcc1e9ea385b33198df136d33f6794b9b6ab2acd3b129649292253c" },
                { "ru", "8824ba6b9bcff602322ffbcc52ec2eae4a7fb3b413b29cadf405c7da077b2334d6cce5f0c1aaa73fba8569694ab13a385d04bbfee6ea66ad2f8455cf7da100de" },
                { "sco", "d47a8cad762392b7ef83eb3f30114285ef3c3a4bc5e9edff501f4bed9212f80f7c66dcc1877983a0d557b80cea5d9457a8d727a1b7cafba585003ca3641cc3b5" },
                { "si", "caec75b1e204810d5d921a20fd05798ae8d1f48360a0139438de967e59613d8bed5df9ec2e629016e460d7ff519879b39587c1ebe9226ed126125fc62491e24f" },
                { "sk", "1f3c0e04a29dd802c23d34b94f145b4ae361d153d714f7464f7f9cfe63b00de7e70880161c214ffb6fe996ca241b1e8dd0ae4a3db0ad882d1739716ce315952e" },
                { "sl", "b35d67be395885d7d0cf2fb9e83ef3483a39249e7d9076c88b3f30cb5a24b5bb4869dbc9ee4cdfdaffd7907f2527e3c3407b78a53ffdee6bae65eb2449e31e2b" },
                { "son", "e48cb15db16789d6d643a3e55d30f67da881d8d6f0b8f726c38543bb8feee33c0711892877e83e1c538c10de443e6a9386c0582940cc208382eb34c2488bedda" },
                { "sq", "7586df65c81d1e33f41d782d28a221df7909ef039671c69512ea8f7ddf50182ca3ce523ff0f1aca9c2101b70477d7fa15bba802d943fd94fe9fb5902fb85b514" },
                { "sr", "2db587c9f79cc2c8b00bb7d49f994c70fa156824248012713c8c7ef307490b358534a7c6b3586bfd204a861de5ed6ee17249f44f951c427d9f284dfcf5faef55" },
                { "sv-SE", "742ee7fd9882ad721558897d8a6fe9a25bedd1cb8a27e5d0b93fc2f465d3d751aa8cc46f3dd4ba3bfafea59a0f72b1ba170a00f0e472d716a0d42cc40783caf1" },
                { "szl", "1b2898917b9d9bec5572a570e9b33a3e7b94db4b606488df29a8797869b50b569e8ddda4fb7c2678e9cfae18d6583001458a540469a611bbf7b82367e3a97ffa" },
                { "ta", "813b83857e5aca60bbe9c79eb5cbe4ba42c452ab3171d07ee6fbd13e1f82519867e0f06fc205ca0b57b685a9e2e288f38358870c952a4c0b24b340ceb531e73a" },
                { "te", "7b7b0079da6548d41798e5669f6331e72cff7cc5d634b83e4ded411556e4df4b03ca456e90ccc91c25c4d01b6cde17038628cf2a0b56c0af01fa32b45215d369" },
                { "th", "8a74ea30dda2a745bc76c8c6f126a52aeabf81a3aef6dd2840c8c77e7b0617d8cc4cc673c50e5121895cf42c16cc80f40c5f032fe78afc7e557c7dfbe95c3f23" },
                { "tl", "feec51fcf8dcda98f0250e205e8f013f29b62bffa7142c15e8cf5f1c5defe8296afe7e4dc228ff4f99e7a61f2044b023735876ae1a5e2636b0a0d82fc45a31e0" },
                { "tr", "a531b51bb208a8c50a5676a61e60e7b2cb352ecf403165e794a7c29400239b1ab5fa271cc53a6fa085d093cc53802513e9c01d1e335c5382264b5d8cc1eda87b" },
                { "trs", "a503d53e9d9049cbb81ae5d7161feea7584ac047771fcf27fd51401e57945ba9440057c1b90d7ecadbdd702b30f940728c6153f75ed7b91e3b4fa2ff33877aad" },
                { "uk", "13c1fdae7f777be920b4b525778d70e593b2061d7f907ea5f1e1b0be7e3bcf0461edc6b4cf062c48cb1fcee802232d612a1c88d5ecd163a41a270b5866c86b63" },
                { "ur", "a1c7198d14e9054cac85c94cf8da2e59c32605c673f626de64e607cd3030d13611dec6e3b8b6c3aa541679dfea822e10361adc1e2d74f014e9200c3c3b8a9485" },
                { "uz", "479022a402c81e3d961bdfc2dce63fda7af78398cb750bc1415f5d20bf4ba2f750e9c3c87dd44d464a65b9ba06a8ca18dc96eb72ba8ccee0a4c74d8241815485" },
                { "vi", "fc7abdb69b15dfaaa88176c998d57dca451af01882f32a499f8274e9d20e3e58abe4dbd366a66e7eba724772ff9534f282fdf380f6ab7048876accada3e42cc8" },
                { "xh", "1d007ef71b5e285de02c99e39fefbf0052568ca78356fa02323db99f9e2f475328a40893464d268934575aa0cb0f8a7f3b6f047a54df350645f30ff89c881ebb" },
                { "zh-CN", "fb33c9631068b808efe0d978da137c1e55857b0642596a2682e3eb9a72393915ba2ef8bd7e444bdec9f31975930cd97d7540bfeb1276f6933c768cd8789f79ff" },
                { "zh-TW", "9b22e35ac1ba0c8d78c915f23664fabdbd7a1f3b9cbe4ca293700e44f9a785fb5884339e54490a307fdc2a4fbaa92c182b62247044b13bf8c52c9ae647f4e0bb" }
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
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
