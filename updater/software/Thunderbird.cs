﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.0/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "54c4bfa544792542ce232c2795b0d7002a20c6b6d221370a43c93da8086283e6eef21ad5404f3d8fae4d900e43801135f68d109c122b6623fdc03a6f9ec26450" },
                { "ar", "71b61281166c7e72f10bd70db2ddd348f883f9616b9e41e4ee7241f5f0badabe2ddeaee02d4f8430c72c9bee4bea59fa49c83d3ae91343fd75e5ac3b218afed7" },
                { "ast", "e70553fffafdc2855722154adc240e4c103fca1e0c75e28834199bb057a457124b44100eebb9e330c8630c221169e78f11a138ca5215e602c6b5899bc10e832d" },
                { "be", "ea868f3df1e7f3970741a378ee3c695f2a600c78bf8110fd8c8c52723a7515236c7a0c5d563997229600dd68aab91d36328d9cd2a9709a15dc32e3f51519bee3" },
                { "bg", "9c22fc807cb9f1e315de900c00e117906542bd7f665017098d459f51d3ee75b28a65c17de9ec7f5e1f40476a492b87a457ac8dbb9931a770f0cb868327336731" },
                { "br", "0579c939c01e8cc7c89525c11d9c6f7b351684964167c3050556a18a80359a73b1b667ba846a08fee3240397d6d69161fc6fbfb3e64437185d84b49a144c56b8" },
                { "ca", "7cfd85617a58236984abb0a06193cde4145cf3a83fd82390df9e3536003cf8f7323da9086e50d85f1384670d499c4788b6d289e1eb3f51e0d39e7de232fb3352" },
                { "cak", "54bf91ab934f9598d9b361390834d6feb609c98c62f7d95537311ace17fb0c45172c388985d2c8838cf746ad0fac31bf25fdd8c07a5d35332a39273d3f975ef2" },
                { "cs", "95544e2c598558975e9a2f3bac7ef0fda50ecce70d96b05c85a06b060030807977fb3e8dd3e35fc9efe2286e008679a8f2e3db47fa38ea2b4d78331f05fc0e04" },
                { "cy", "c17bde4f18e4d4f7cc00cbf90b99424b73c7ae76869c69aa3017fb2619850faec79b3d0bf1e32d5cee252f6d5b2dea42693ed26fdea74199b33e980e4f2a9f99" },
                { "da", "259bdbbefa848cfbc72db3a21553e910ed1413ab149d86dbbd68a5ee0db106a3ed00097bd5056b81627116ddbe225bc3c1cb9065d434d0f46e320c739cc61deb" },
                { "de", "427bd1f1ec6fdb50ebb0df88289178dfdd747e435fdda0dc18479351a613f42d13a752e5a9c6796019a933bdadb0bf348c5c39a37e59f5a786036af474f0afb8" },
                { "dsb", "1838e097f7d99691f8a20a551062671440cdacbbbcb2a8a58a5e036852811e6a7f7ae063e8abb143c3e92bdd65e91fb45d2708c205de15e517417a49662e2e7b" },
                { "el", "2f4a92816436d8f64d7d64a90dfe8545c76e1b8c9d87755895291f41df3ab4a0706c27c346a848ced09e97134d2f639ff073023f82814e5e6889a4733de7437b" },
                { "en-CA", "dcc66abee5849bfd3fb55727b9e78e2a256eba093fcf3406872d0b51cf1dd476573c6e8b3daa1ce1d33c990438e81b1a952696d72ad2479df682552a22d4613a" },
                { "en-GB", "c93b752dc07d1eff97fa8774d5f3a8db7dbe0d949ba0d4a96e5a92d722db48e772d8a9fdf294a797a67ebebd360fcce4c7f0716736f0f24c2789e644350a6e1e" },
                { "en-US", "af2127ea3a4df4461c355db657fe23a78550d7e2939580ecb34e92d0d6ff8998b39de6a0c616ced2cae0e611532723d9e012b317939f17d63135dcb2672ea94b" },
                { "es-AR", "ad3ae519a9b890f90c8dd14a7d3fc2e0c7678c31efdb315f12c076a3018d0257ec62b9157057706b6b02a4474b303bf2beafb024784eb9f587a6374161e3cd2d" },
                { "es-ES", "ff568ecd6cb07cce7ad3a459ae85a0d83afc4c4d94414b7b177e1a5e2c561d7cb383dc22aa9db45a706be963f448bb63c68ed5f537e4273c2708f402822addfa" },
                { "et", "92dc6366bbd9054874615098ed391e1c0fd64cea619690b60b1552932e2dd3fb02d221503e3cbd7931e64dd6e57e0a1bcd045e09684b3292e5c109cb6d680ae9" },
                { "eu", "fc418b7c48835627584471d2a101578a9e3178bc5248e04862bb3b3a1f5c14df9a43fec115872a1d0abf9bbc5e371e1d9fcbbae464ebeeacbdcb0a9c5a24b32b" },
                { "fi", "063e3d3f1da3e95ecd0e0ff91a9368aff758b3517554e081b817ece40db94f4f744240eb29988382e48155b45efe1dd3552703d3990f36a35a3aecf93eb883bb" },
                { "fr", "85dc7afa1f84d2941e138517c685be7b1544bc3f96140d15f077133781b790d25b3f41088931cc5e00e5233bb7eaf864d3a36f6f69b8e2acf6ca67652cb39076" },
                { "fy-NL", "97a4517bbe46aefcece5bb5b78bf66a5cc34623da0eba157ea14e4972e722c41e62d72736a051628b9521a5cbf54208535b8818cc8bac1bc23b761329315ec70" },
                { "ga-IE", "dd9d2409a6c354450a8e08ed95f50369578ea6a27b617e28f2932435dc8105df2aa04b109c5f91a6ca6c88ab0e9eedbb432ac55f1e9f4818bb15eae6759e7e0b" },
                { "gd", "ca389251cac7ef05a7a0fac132428b68d5be75dff4c16e89b5a4723222a7322dd587748d495dc5f1a3d9ad7ec58f5c878b00ee73d2072f899e124b5a769260a9" },
                { "gl", "74e86c7976879d0886778a6b4e900c5256005505e9ad022991c629e6c0f15c6aa66bcdf428221ef4fab14a3504ba255d264910bb86fe2af161daf799ea93ec4d" },
                { "he", "4732c808252dfaa28d781f1ac3dc75950727fb56faf0b14f71e7948850e50fb3474c24441b1c0d6ebfd59b996d6596cecc1fc9f92f17fe02eba15574f4c6cdb5" },
                { "hr", "a8c1f4dda74c3331a124aaae5d330e99fe89957f64cb63e14a3a47486cda79f8d66ab8cd193d6dd2cb8b16665067c5b10893f2b0a0c213a35f7929fdbf1b95f7" },
                { "hsb", "3e6d047475a30fa5d9c7a787d334631e176c7899b16fa129a02f700910e7d7e904f65c27cf502d732df2abb821245df644059bdcab992f7adb8ff9f9900530f8" },
                { "hu", "06813ef70bd20efbc9edbb40487184fd9b89ddaae7fdefb50fb3ad595a65d16325688443a7c7a4fe6c049e15a00987f485fbbccb7b76f7a1cde0025bb073d3c3" },
                { "hy-AM", "d7c07faf21510c4d6426659cabf94b8b43aa584d2e0c912ee7fd2a59b632d8ce5cddff1a0a34d3f17cf4828a4f143a1d5298049fe71cf2f06eba76041b936a9f" },
                { "id", "e54ac952f7af4035012bd5bf8f8c602487ac509630f94165c8fcae202af7f48987a05f0f923948a35b03795657542cce659d481bf63a6337651072d0f9dc73c2" },
                { "is", "e045358892869451bfcc6b51d9738dee6191e9c029d25ae35aee1d0f1bfeed29548fefe1245317d60335cd4ced8bcc4bcf4ea47a6f28c7bcc46a89197b801bfc" },
                { "it", "17d9f9317a045db87228f9ac7b323a83971fd97556f9ae4ad67394b46d7414d2c8cd8892748012f81f6327a84269e5404c474527830e60249af18501c23f0767" },
                { "ja", "e950fe925988a79a46e9ee34cac519c9aa7aba364a70152f99f00ac217b1398865818e016fd14a02eda96dad339e8a8285be15b085597a615ef2c25421d9dc6e" },
                { "ka", "628cc20ca52664117786d0dbdf6a921742dbedf45f4aff2fa340ae7c5b37497fcdf4a6601bc307e17210f7a48f18bc7ee41b475ec880130f5ccdf62f81979228" },
                { "kab", "8614534139d0922def82b8ceec415816e49f1ae7adb7d48be43f1d66281b9df2c55dd3e3a326a181063c622f898346d88cdeb6165fba7ac00be6b3311b50e550" },
                { "kk", "69cea48ab80cfeb92485b0966b0c4e813a045ff7b104a8c953272d86122d36b5cb9c9510999a6f6b95df332f0d28aeed2395c6726026ea60ec64038ed69def61" },
                { "ko", "2ad6a154384d8781290596fbcd763a8282024cada935014037803eb0528f4606ba6f0e06f237aa4efcf1f689849565930c5cb2eca4eacc87eb9b9071d2839725" },
                { "lt", "b33fd2a592e552ea55629feade2ed668c0aab3820232463b5edc7e6b08763ef236582b5eb26c7f958e13c24fb62c1dcbc79eb589e81b793c9e941bd8bd910110" },
                { "lv", "5944558fa5be999691382175c710119f1e4dd2d52e6656754cd9b329e14ff8977e4ccad7262d841c42d532621c9d8ba905b8f99cd5b67f71a924d32ce3a35ba7" },
                { "ms", "d7c22e3a34a04a32d9f96b6f4adb918699d29d891feadb9e90fa7f19b032bd874bd4d2fa86baedbeeee8ae4aafbc756bc7a5d8f073996826e28de5fde46e7370" },
                { "nb-NO", "2fbe800c4351396c4f1e98924e69eaf3dfca75d7d32c92580d5141afcc276624ec071061156886ac38267b6eda19cef9bc93ae96b562d9f43ac820a89c583276" },
                { "nl", "4564dfebcf771f930421817df029c081fa33d44443f1628cd25b73121848413f0e5c130f86d6634c02852f65d79d64a1e64fbd02fef3361df339599a7ece5a83" },
                { "nn-NO", "00dc49077f826afe9e89775a035d1f8865a4c5bea7769f9558bacd9c4e77b220fa757a73894db2fe9acfa8f9a001e0bf22d31d25b82c34fe78ed3fd75b3008d1" },
                { "pa-IN", "062877dc74cd8de9b2a1a5b9c1c0975dd2dd242cdb5d324d61f8743cf369c177d394dbd65f501d28fd264b5a62d14f0c712aadfeb091bedea952d70cb86d3c86" },
                { "pl", "a59963649f2043ad1b21f58777db5b56e77df2e1f2f0677ea600fb761dbf9962e9ad470ae528b63b0db6aae6f016bef5065d543919f535e22e18766ca27d9751" },
                { "pt-BR", "1cdc8a70f81e7335a7a90ae85a12c7ab5fee23ca443b5e788ec85b0adf0140cbe7ce1dab2fd990f50c0d2f512961a346c0b42579fff17fd10be287da87926b70" },
                { "pt-PT", "4523b6a58b37a4a1cf18e053e640fde1df597f146798811a43126ae894250fa1481539fc1fb85a666cff5db6d68f00f3284ac8a99e691c25dbb945525ffd8655" },
                { "rm", "7aaee1df904539e5cd784eef869aed853fffc61073a6b7eb32de998c1ddc330870169f709d4af4f91213ea0d422135b6945f8ca8c784dcb8e0edf563151968fb" },
                { "ro", "c72a9837ff179195f59cb23426182c848f7b2cc281555d35446a171c7b498e581c60a8c60a8d428accff8b1ded8d9d5432dbb6bf7d4875681169f4eb2a4f8145" },
                { "ru", "90406e83b6cf896fdc6841a21120a6011a7c802b24939eeaaec68b3906cff8378825fea1bbb6f5b42d1a49cd4b663296f3399d1ce1ae04ab59641e4abf9447f6" },
                { "sk", "a52394029c75ed31edfb80500566e1056c480921df9093d9f6fb3903864d5e56ede6872cb2938251396fa1ad0a155fd43ad3875319e981ff7c97152b7986e335" },
                { "sl", "6bca94f2456af66a02805dd4e759c92b675e3559227d7ddf7a01bca4b6e833a0308fa835adb13893590c22e4be0e3688ff9e72cf26117b4eb5b082cce695b1bf" },
                { "sq", "213923c4b39528b67eb43262df1bf00f70e9c329824f2ec90b63d082fd128ec6ef9e4635b3893969ff590c1e11d1bc5df2503723812cb543f8d709e5ee80a9fe" },
                { "sr", "90b9a39e8d19bf3ca9fa3f5bd6d7cb3d6114ec8e1fbb2d7b2ac1010ac28161cf7831020a62ea27153b6af7e711ad6d91d299796098e209d6fa96ed3297ff6824" },
                { "sv-SE", "043dc3acd756a148564695edaedd24bd66361beba4e6eb83999824d3dd9be13ad87fa0e5a65098ba24afd2dda5a1bf1f99649c4ac2a157f521864b000960f1b3" },
                { "th", "9bb46e51a36b2e17a5291dd0f1434a2564de2c01a234a40cfdcf52f0b807ab36e655b887b9258a60d60b9c7a1ec4828f0969381043d44c52c05ac4a2837dc77c" },
                { "tr", "c3aaaa047c0b284fda38c5157be6939bc2097ad2ee46407fb439b997979d66142b8178aad0d92a843e3c783c986f366e9f21d6eb47ef4e9324b2f88257a8abbf" },
                { "uk", "a67dc60bea0225b7a52e38b5715a26a7c3f57f42853b134774cc66ff784cf276d7ff30a24248ce56168c34249fa5377bc7ec887783b7a585136f081c325d417c" },
                { "uz", "3ff8519544634a8dc5fc2c449fd8c31bb78cb33f4375bf0ffd80a2ab414a112551ed1c0fc2f2a8dd31079e1462310acf78cb39c37d61de7debc66b214d73acc1" },
                { "vi", "19aa214f7c228e96ac6cf6396bf12abd76418c4924895c30d6c75b708bcf0294721a0ecc7d84d683011a6e77374ebc5c61a036e28d89e5c8121ec983f9db67f9" },
                { "zh-CN", "fc320c4d413b334fb6183a7ae55f9b06292e36d2e9b2f06fa7590f63896bad20c01b266d8933a057871305118b0aaebb50e06ca18694581a431f73d381304fd3" },
                { "zh-TW", "8ba08c774357cb54eb214629081027c1a7d177e5062b9e10fdaa8a1a4976decc88975c16c0d1b3e9e6ae4ba02dd9e9360d479bd21cc06253599b15e41b35c1cb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.0/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "6c9c3b5516c044f4187e4481d37a617f220f6cd11b7e1e9526f1bb07482fc6f5785e5d24516777130221bd015af464a22b75b9ce133e0dac896880ae2a15f8c3" },
                { "ar", "ccb3aa200aefdd2840853266e14cd99cde6c64309c17de0b98f6fbc04bf8462c8aefbd173785deb5bcab888a398830a056251ad58d7381b8d191fe7cfb2621df" },
                { "ast", "dc28654108a2d03c0ee9c2410c0c8c2967ed9a80daa736fff832a44788b4a835df16c8ada2eb6ad0226bfac938b7d5027dc3441cac0837553e085682c516a32d" },
                { "be", "082bb2412ea623534222f8617ccb4309c95e127c290b609dd3d0dd8ae69707e864e38c4d064b9ff0235ebd8396e49f0b92d5eef6ca5ff867688eb684d254f943" },
                { "bg", "f1473250e82dc635b3be6ecb3506f06b97c7f71e8e3952249e4cf6a7541c13eed982d754aa49719f542793eab67782aa7c64e5041a454c2b4cfb13937b7c4e19" },
                { "br", "428600196955b53d109f4b4f002d9e7930b041624b59bcba55deecb88de9b856071a4351a18e2c0bd94cbaabf1cfc8ccb18c6f99507ec0be51876b845541a99a" },
                { "ca", "dfa376b8067296bf9fb08b957e304a2d29a38d738b34bca8198e8eefd5017b55c02ca9f9c0ef182fbc33468a34a49600dc2b9b6d8b710605f963b838606aec82" },
                { "cak", "11274d96d3b5695ff42ed3eda8df9a4b15d5a0ba070b199eb5a7dd6b09ae2ff0a498ab18a7a45a8a27ef6be14d3e2797812ecee62910f1e2f5f3e9006bcf5065" },
                { "cs", "a7c919bdd5f2944cb65aba4fd6d2d0c8a4913f1b0ea11ec01e8ef244c1365e0a241c7d1352cb5e7617bc62d00e142cdf0b8f500d0f8185cd8abff9ddae5828e2" },
                { "cy", "9e4bbafdff5360a962e2e83fc39dab88ffea783eb58ee28ab0f5efa0a74ff553428e39f6ee1a39784e5e71a23fc87b2a30a5071a1809075b3aaf5ef221ae8be3" },
                { "da", "58418466311a8e80e3c6f522511a193878898fccf321b9a5485e54978bbe594a90fbd6a4c7183a6bcb3ccbc0881fed680006b1a1be908342b5cc44283ce3a387" },
                { "de", "43285efa99ab768f29f9375308bd50301b47cff0155e303a9b4b7179c39413a1e3f28ae8309a5e47021d1f1ba55c6ea8dabfb4ede01cd2b7cb1d2bc03d4f2cd8" },
                { "dsb", "e962ea0c696b8dcba102b4b2f9ba7e92feaaa8428c6f806c87dbb5ddf73656282922e2dc68ea1c67580c016de2cddc143b3271427f55f6e4e1cce79e3b1f6434" },
                { "el", "e520a9ac1da49940d1aca0bbdf4e2c78159a22d4e55ecbd1cff94a6fb5a7b849cc7d4e764d97efe6c9055c5d6b3e0a1b69074487e6b865f489df5b5a7ff88593" },
                { "en-CA", "2a1551042712b76a9082377cb5062dfd623fafca3611aa7414cfa6c062c3cef2ca1c96f9785b14eae4525ab777138e48ecf9b47780a03d5550f3dac82fe2984e" },
                { "en-GB", "6ea1a73afb20a59b975e7174246860a31891a8bf88c2b40e4d583a5f5fa67f5a01931654904920ab3ac8206ebb2daef735c7b583f2b5233a98b9db35e29be280" },
                { "en-US", "a5b25423611c1bfb1eddb7aac861040d486365eb1a544c5c518380b906df3c14a1bbcf48bed03ba9eeec2b6587faa5e996d06ac3bfe79059e7a3cbedfc912abd" },
                { "es-AR", "a00336777ae59f8d71c60929b1a13ffc6b1a11daba0fb14d6ff3ddbbf3a3507dfd971f6e0d5b781790f5d35c91aa78c8bcb661a6c10ea8b8eb332dceea4611f2" },
                { "es-ES", "c07dfb6bf8ef72807456ba1e91e7ba8ab20dc9b00db719b3b17eb7627b42a8273b7396ea8b73b53c80284aa510c0c801fb21db5aac881c8d8b0a3b8e8d11bb72" },
                { "et", "0759032ad249a113d7da1c5fe2d2f9d073a80aea3ae57256956147c116b6fe518ac024353f6cbf22924fbb7b6b95bca92977a6a70a95df064e4a7713394cdbde" },
                { "eu", "48473df08664b07eebc2c72ce6764c5337be5f5785d395c598112528daa6761afda6eab196ded6eaa7ad3c6b6cd321a84e19e63682b6118c7594e890b09698ef" },
                { "fi", "89466ba7ed7f94715edc9c254945966fd8f49615d67ed45b90d275f4954d7d59b9e7511ab52412b2dcfab3fb30384a6f94b49254be067fc64f3b3927e1c22cc3" },
                { "fr", "53ca2ed70be6bba05554146ed7d37c87792d3d12c2d5c7f366cc91378a1e613dc99bb01961b4bcb0ddf1040378ee9dc291f713e7521cad6a2bc9387e0b1d26ab" },
                { "fy-NL", "77ca684442e7f944376cfc0b91972139b396131ae967e93876f9c575ae083525cc03e7167fb721b7fe65ef90dbc933a8a940f892e8198cde6014e601bf5d4ae0" },
                { "ga-IE", "d03af5f55ea8d1ca1910b28eb0ff390293b0f0255af5d2355b7fc9e50a91dfa878a4037a9b50abd40b463732b4dbca55ab94f24bee16a08899cdee30e9023883" },
                { "gd", "402d4d9ff5a674b1a092b339f00b412b96be7e1aac5e6b1bb58e11836822a988d55f33b3ea7835355983ed7e19dcd651265d12a41b1454eaa381c9172c9d24ec" },
                { "gl", "d3108f23b72ed74990b9483b147d862c48a04ec272d7a268537a1d344d93d46ab5e0d02b7dab9c3cf251f220ec09803db3d20fe37a310aafb61777233aa96bb0" },
                { "he", "8e7fc0939dcdef238dde0286eae8e458530b9aee44ee8217f09fbd9d9fa856a8f8ad2b086818f20b5c55c5d79f9d3afaeb06fbe09acec2e672fe3d3607b9fc03" },
                { "hr", "224a04ab3bd58d10103c7f8c75eb1238d7157e60bffa56f9df5a054664dbb9da164ba95f52507d41ef70e541ee1f808ad868765ca21a8e57ab9c893bfd0145a7" },
                { "hsb", "1da718ea4f012a7e29a43094df67e727d95151444ee5de6349e97b6b3cd55b47f6b3aad04c1015107ae642c1cc5ca35764795ef701ebbd4084aa409a69061afa" },
                { "hu", "4c9a8ab98df4be1dfffefd6e6ba3ef4404e8e0c6bcd833fb854b6fab31afdcfc6a0e1b686d13af5b2708e0edc6c17590a6891db843272e3b4b2da295ba9e7a00" },
                { "hy-AM", "325bc8f94361dfafa09c77e3e8711d286bea0b6d31ce02608a48d84816f672181167ca036a81f259a54d9b64e9539946a8151a8b7cd5ac693f13622f4683aff0" },
                { "id", "41a14f16536e64d91ba11cf57535dfa06f43479001f7a0dae0c0a796a0302a3523a01af733293b91dabdcebc7b734c2385c7c8b5b82c8d96d5dfd7c9a9a5006b" },
                { "is", "e16962e78e99017e45a7d66cce856f8e5ae56c080073ea617df1a1a5f8681bbdf02a3ffaccfbd4fdca0d68c3886f8014f74082e679a03f6a0985d7152dc24b67" },
                { "it", "626d43f3563f500fda043c979bc8475c1ea46c688daefd7d8b9e3c5c9e2a9ad810d694a6f6c92304cf930ebb31c93487de5dbcf193fee060760413c5fbd7daf4" },
                { "ja", "48315aa95a5adfe34e48e9462f4f66b534be2c58ddaafd631a252e5ad7fd676bc9f490493a21c46604d18c428ad91bc49862028f8d419070f65b902198ca6bd0" },
                { "ka", "f63c02ea8b6b279e4cc07fa56002fec096ddec1f36ca29fab7093db8ac51f09afc2d687f3995f884d1848c137ab68b2f99a84e42638f0ae77bf02a9697ba6237" },
                { "kab", "033405618b7d6d74e8c1c77024ad99359342a774c956360706c8ec54c98405cb1a9b38270e9f774f60fd527c1844a2731293c8f7b555f4407e554becbc2a818a" },
                { "kk", "dc3a74128ff243ccd35e44b2fb113c3285e0dff81c9ae9762a1426800525cf448bb5662be2886ae89c3954a8d675a724fbfa1ace933b1774408bbdec0cd2303f" },
                { "ko", "f11818ec862f9c9f4373bb9f5a2bc82f33f6939e0a04b162aac706487afb51ecec5de029e3509cd877ae6476e637a727b124ba8e92f7352a3b211cd102cd6c7a" },
                { "lt", "70ca6caa1147a567ec0c9eb4bae327e4d43305ebad68033e36b65b747275c91b48ed272dc066d5935686020ee1aaeb6b604a687d13f385f111fe841206b984dc" },
                { "lv", "0e41b36d9b089979dbd7076553414eef62ea2af835a9e0b310a944798fc32afc4b64c0910f3ac83ec1a8b2118a945dbd72b7874d33166a766513a7a44ba9267d" },
                { "ms", "6f4a426a9aaf77b48a8d458ed5e092a96d1a247967ce28fea1dd906a9d5247c51cede876836b8ce3376e96b1dd1d189f407b3202730a58835727e726a9788b36" },
                { "nb-NO", "0f2086488c4a77da6c3edddeb8df611f83ced27a28a4f70020522bbc7f4a82afbc3a5494fc37efe409e8f271dc71a1b1f6b38df36153badfbfd851388a5e4dfa" },
                { "nl", "66c9e89d908ad1da8249c36eb0d3db625ebc7aca98103a149431349aa694cd5b8f5241154376da07fde7a51e381c20cbc70972ebdf02985c98f349fbd55cb69e" },
                { "nn-NO", "550d5397db2967685c06105f274f493f5dbdfd4a0838d8b427b45db3f9b737b21d6a0b3c9e40e39e2f47512790a40ee78567a4c8b2c90ba861572c89fc53242a" },
                { "pa-IN", "db5b95178caf546deb1341b5066c942e51c353ac2096559b0602d044d978468f21c850577fb264418c2e9b9ede92a8a4b65b0b07839f8c8901b6756122d4710d" },
                { "pl", "f1107a94d9b9298a02ccba6bf182a8e7ace52663a7c2b3650e0a33ca1234416943affde5dfeb9ba148d531ae618d85726892d221dcf1e6cc73b19da6a3def134" },
                { "pt-BR", "8f2f0d50793cac1d3c543ff694f39f464621636abf0d0397324a0a10ffd41a03bf11157b4fa671063597e40f95f417e1bfa7c0c116b6f1397dabb51b1a63d4ab" },
                { "pt-PT", "9021efd1db28ca0cf9236f3a6d7a5bacddc9008984821ef6e529355cfa87461fbe66e6064677b821f05acf08174c38812398fe0bebda0f2e95bfd59b9026bc67" },
                { "rm", "0d1ae294b24c2468ec652edc1abe4928ae17b0022bb4bc65d6869d230a80dfdf47ae3e54b9bbf2aa28c67f5b0e6ec2d7a4afc74b127a6509bb719d39a531609e" },
                { "ro", "08cd55ab701a68c8b02b56078183a53ec7488e4dddac6ddf1e89be2a9ef1ea5b60aa79f51caacc9b89ce09985b7ac1cf93f34f2d402d5dac45c7481ca4cce1a9" },
                { "ru", "169c4b900fdadd8c0cb445afb78ecc79ab4ecd5e80e35d834afb1a9e19c51fb7052748efea14e3ea0955909e44686d945ecd32f64212e9b2d64957c461da3afd" },
                { "sk", "c3cff12c9d197fafa60171aab9f6efc4fdee9ad7d10f2492a6684f3fcb8ccfd61a09892f85d51336bc759738e702c06669ac047c9fec80fb3de88e51e41a00e3" },
                { "sl", "130eda12132f1f6e385c1024c63e10f5eaad3ca8d443312a85920159f2aa1e0ea44833b0bc28161a76c11aa4b180026397a6909cf716f01c0500a824f0992754" },
                { "sq", "fa5e6d7b5ea5edc2cb5a2512504f29eb618b9ea79b6aacd5db20dd827dc1895d23c6d92536344bfc1666c26ef72a7a79ee9363e1989892ec392922197ae328bf" },
                { "sr", "d2d4c384aa6d98eea97f7d503c9b341a8f82f10323e2b142aa34200ff4493b9fb87109879a611301e3408bf1aed8b3e1a9da3ee127a379044a34dea59d87d877" },
                { "sv-SE", "8df564c37e85671ff570d82cf848d4b80ac2e5ff2df95da5384ebb9dbea9922346d2e8af5982a8d373bfa1a061ce1d89764688554f4342e9d4fc3a5452c26788" },
                { "th", "fe0d8a104e363661c494691962c65b57ece4e6d5cb9ada662ea1e4cc8942d26be090a288f3921fbb5ce7c2ef52150ac735479c13b3bb6bed15c73ee4792ac099" },
                { "tr", "325d16060a616648e0891fa22beb11e2b88120d5852115d85a4f2549ab7b4d30abee363b01513c80d32d6e874c77cbde6b3c8ab006357522d1364b96cad4ab3f" },
                { "uk", "9cdd34616267c12c052c3f8586604a91fb446970b273ecf2b319f09a712077365579934ae401c608960a33afffb856816d72a1fbb306f703719f1a655738ffdb" },
                { "uz", "0b0bf82e10fe59c9b46a0282c15536991b9e315be55d9a1f79f3ce8b380a1e2f8e215a085ed087fd7360d28b1db963492893e2d169d0aa5c79e921b7b6c185b9" },
                { "vi", "8f7bd6780648abf8ca2439ef29cafc6337a510ebfa27515a0459c065872361ddea729587d5c642fca7e7b9aa91d0a7dd6b86651e766e533b8767ea31472eafa6" },
                { "zh-CN", "2760f1f3034bae260426a90bc30fdca19236b4535b933207ba791c4429b651fba3728c086c76f73188b20303228f072142ba12fd67c17bf41e840ae2b55e4cb1" },
                { "zh-TW", "7c5f61fd7b00e82e3897d2ee3f9e5572eae417f7a3aad54a547c76a2be062ce45d31e82c561fbbf34e52dc47d776b96f2f4619c84c5fd666fdab7fb8fad59d20" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "91.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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

    } // class
} // namespace
