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
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.5.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "5e42d993ab5a13e3be376012cd60207d3f1e99d8fd1e00f6303276546c9e1754f6f86f58a917f2e5e5b7c96d6e11e01f3faed05611cb32a3b6b929dd97c6e1be" },
                { "ar", "3979c54fcc122de0d2e729237ec61eec79c0f47be7f078b9b7029fde21f579a1a916ea540c63d6ce3d0d7b2ad8eaf32c52c3593cb4f41f8fb609a0ccb8a8b450" },
                { "ast", "e80e97a22d7f02d4d20312fea724eb5b258caf661dea63b722e226262ed1bec3dadf1938f44b9c2ce31f80a029195985a36d0fd3099fc2d4cef688348dbe2a41" },
                { "be", "bb7164e3717087cdec5e3e7b1cdeab08bcfe766f416b4bc9c2c936c9dc77fbb06dc9a95e430147ab3e1becd3fbff652e64d18d664db166f521e88e82b132df16" },
                { "bg", "f7e8a87a0b749108562fe4e44aae58d41f60367fa83fa47eb6cca61a342f9dddc6c80668df788910e084985c7dabe607c786fc92f65222de7a8d5ec1cd2b00c6" },
                { "br", "705735ceea1cc9afe806cc9391519bf8c31c5e71bac909f2df091defe290932f5837e40501026792ec4b3a44db52dab53e239aa90023128a8c04a01efd08c390" },
                { "ca", "5320dd6d46c39c7458f1381fa003ea06e6367bf349026eaf17ef1573415b157ad71fcb5117a92eb3324ee52639de2be51b4dccf70806e0700c14e63fe5832abf" },
                { "cak", "56b5046563d37e4217ce92a3fb2556452f49b57718eb6ec713df0ff06e12c89b6fa08549cd08d0fa5b3224bc508378c2ba66fd19e0ea6b3cfd2fe59fc72cfa98" },
                { "cs", "63583de9cb0310d104c489a1769c0f852b192c999c1d6247bf2fc341ada92494b159c03c08e52bcde9e54e41642fc95b00b06e125db3c488850f84adac2d35b4" },
                { "cy", "2569e6c00f04413fa39e6b4f6bb0f967e0a7f2e7552dc37b880a741bc5fab86e8365e8ce7fd492b65449fe7617fb1070166ffdde459f2da95daec013f75c516c" },
                { "da", "78d114d37757e7eb8823df9d850697944908c0b68596ca6ab7ce67a2137d159e3b902f4e8cf7d10905b5de0de8943d54627cd4a945602bf880847b22ca6b0c40" },
                { "de", "c85beb1e484e2a46f92b6992f6b9a0d95147b20505b4c7f03fd893c20d5379e503d49be8d39676383932274139fb5bb9976aed7fe854abe483c43bc1ed9fabf8" },
                { "dsb", "9191825ce1f1a4d595816c2c76da65ad9c77f8817ad11c65e2ed5718ef00c68d2a23109c9d562987f32c402802a4330698befc07c812824f1f2e1d894677a34e" },
                { "el", "73a8c6ee2d4aabcfeacb18e79fd11ff91116cc5f73bf4771d24455ae2285764f2ef7c0c5271dcbd8626859e99c8ab092c2861634f9c15cdb90bb4c73079df6e4" },
                { "en-CA", "cc2dca3bc45208f19c7c4e034eb89f839b2deb4d9baabe1ebb07363fb4f75f04043ab65bfb0f8826141f1a29e0b2a1d9f1a515e18c9e31be9505c5e055ff658f" },
                { "en-GB", "e8274cc2e4058da860eb654f077cd8621c0c46df9ba80d6f31643a4bdb5655052885ad309e51ca6573dab8486c9fbbbaa1a0fcbbb067f6ca3fc9c5b683a6aae5" },
                { "en-US", "74b3bbfd636179c1d14134568dfe94cc857fa50e9972044d28fc4ad6e8cf5ad9d95a3a8b35ac138d8897a4a360f2a61395506a35524575251b7314b8e4028c3f" },
                { "es-AR", "a7507c6044f3eb42222930e356712a22d0a6d99bae4e16f3be7f8d2a85be58ab1267d32299bcd984c9711c401a6952851c7b25088aead4d130ce081d98d6c138" },
                { "es-ES", "1338bbf21ec3e318730992ff66db394f856b72d4d80eb7a985b4f2ea43cba5ed2614d9ce9cce4c2fd923f8f6c2ec5215e2d3e290044a3bcdb821fe874b62cc77" },
                { "es-MX", "73b39ed69b01b89e96b1864447d87a411d91b525cb78df0d4403b00f88bb100199cc9e709c2f6999ab0fed20391f0b539c8026df9716af38aa90a4bea2d10ac9" },
                { "et", "58626ddab424f9f0cbcb64910c82ed12e8e9b3d683e7151abaa4a1d88f85d15130c3e9615040cb5ea846fdc1e56653a751f6a4d78d6e7a5d5d7ffeeffea279a0" },
                { "eu", "cc9c5806a6d2422deb831516f83c1f93213c4a583933d69a5899490857d7f8df10e0c2a9a63ac246bb2541b319f5d2ac30873cb28dd6310b512b70ea830fca27" },
                { "fi", "54d2ca3d85bc1d4a999034bc613a8f6c5e161f05c89d55e0f800e12e002c5546e067d420747a038a9adf7c00cbcf6da2ffcb508a77402279d5283ce8a434107e" },
                { "fr", "6ee9793fb62c0977fc02075e2aced1969ff1a222abc70bf3e430298f459d2faabc67ba497bdddba7e2a236ba0fc7e6e94d03ce661b1df34299d0bea4594f7bdd" },
                { "fy-NL", "fad47f9c4e7e7d6154a1b9e5217a42ebe43ebdcc914bcfd8bceed9b50afd2acb4d5a477b18921b2bfa2bc890324d2f5ca8596fd3401a6c13f2f9aae669bbf5f1" },
                { "ga-IE", "38f77a7f690b2d0dc15477d2ed0995cfdef8f14764fec7bf74163dc0c0b1902e1342e58371c25d3abcebbde42f8ee963cb2274d867e2bdbef40805a5b9f2906f" },
                { "gd", "01f076d18b63f676abed42ccbfee4e795af34a180d1fbb9931980db7b8c3b640e412ba9500f53d9949e9fed091ccd3fccb6a5f2d1216d949c1b56debeb3e376b" },
                { "gl", "f159f67d9cbcfc4d181c5d90c5367f016fac29a6e56394a7e749d9c18affbb2f60f2c6199547d1a19aaa32d231137dc2f8e2a5b3cb9a8091d0a20aa648db61b0" },
                { "he", "fb0c18ba50ae51d07abfb13cdd9b40b666dd42650a639309f8de841e47002dfa8ffc2afc4e58f5678a65b4b18a959f96bd5a9325bdf26dd0f8c052447e828b45" },
                { "hr", "62309af052ddd6341b5738c5165ab491a97acf923861815889e3ccfcb7eb61f3d03f3d3dab155a817c6c6c40cea97228424ab2dfc1e539c3ea0dee63426cbda1" },
                { "hsb", "8f904e1acbb2d05f4b7d9e6e8c0f4c8ec04f0a55095e20a50a633fa27328bccaf917b0e38dee8c44b313adea3de7e1ecddf8593728b85317add9cdad084b896b" },
                { "hu", "ca5b7658b2d13e61b896f2359e2ef59740e0fc27582214ccba9917540f5c255c1e277d10d4a081c8b52d99dd1bad82ae53f27580bbfd614122422532400baf48" },
                { "hy-AM", "d13237503fa14787d15546ba2b96cc594ff9580407c158dbabdd7685a657ce22843f52e07cd9d7d7b2b5dbdee6c46ae0af46dbf5d1cd5ffc366b061f7277d211" },
                { "id", "a03a7073f2336c64b240f836737e8267f9d1c4b98f9d541e4374530f35f709c49f5000d5d87f907bde0a4d70bfe9c5c3abfe2fdcdb98565e84bb6ea95dc84a26" },
                { "is", "1d6536651151727b5b448ac5344bc1b6a4ba9ae350b87ec200f7216f04a5f1ee32f7df161805e453866cbce396ce47b872290993fdae7874707564ccee55d340" },
                { "it", "d5500857b62b2dafd351e1d8899d63ac6f7ad794ed17d2ad739a3d67b58bf958d3ea6d93f2f93b3ef0256cb8c944950fe891e02ccc28f1e84fef871460be0981" },
                { "ja", "e8c97a4bde87d741f8647f15e0a1c3bd4966fa25106fc4955a453a39e3e6b90509da6209c6b697ebd1382198a3afe467041a5c4dd62e59e6a218295ec3ac22c7" },
                { "ka", "92b5cce2344dbceeb4135616fe375f3ca37d4d7b6254a45291d9caf9cb20eab6615249a98e8bb15da0997ac7298e4a8127e77181191e20cf45afb423d3f2b00c" },
                { "kab", "926495d810dcd339f3e493e7ddaf7c4eb1fcf40e99a7e6b61db188195ff6466a8fbc46c04d94bbbfd1ec8ab5900c298ab640c4dfb5679da84b14caa8e04de400" },
                { "kk", "fa42fb9f46f5ce254af7a7e6b7bf68841fbd06d1c62175aa46cc4bef9678bb2f02d49ff8a59834e4aa21eb1cd8584fe153a060cc4db47152333909e95d95437f" },
                { "ko", "0587cfe529ec050ff6511fcf3877d6a8c3363e46ec26f8378295b6455bb3b71b2d9857a3f81b396d6358901ce52588a0502fbf558be843913f2291aa26060d71" },
                { "lt", "7ef34afddadc798f04b2c56d48fe7256f45b22e1aa58d58759d70a5a3f87b062820617c8400115e67b1c06cf54cc0b991e9486ba01b1782883a2096bf1293cb7" },
                { "lv", "59a4cf2aaa901b8d2c827d284b81e585ff6870bb1acacf393725ccee44a18ebf138dcd15889f20d8681934efe81f78c30ffdde2b8ffe781c035d6ddb16b230e1" },
                { "ms", "1f07e2a5771fbf0aa1d37ba60db7525b2608dd940880c613351f63d18a8ec0c5aa3853faa05fcff76568bb6beeb8f04021bb6e4daf62ab173a624925dff8e80d" },
                { "nb-NO", "5bd2b9598a70a7224232f0966e249dd3f4d2c638963c1873a222276c015532101842e8f6ffd4ce67c0a0a8c386c3a8db178c26b935d3a39e006f1ced209ac56e" },
                { "nl", "1df209c62673cc6447048c3d06c7aae849d48b41ab7883d2ad88ac0f31c18749d11cb88b4ba7116960e6d8d75947d140a0e453a6a655ca7c2f6e37e3b4721d6e" },
                { "nn-NO", "e06eb1d50b853e4f441542c72003c4aa25c4898ad15ff91a7405e6662948b6edd7caa756a7abc57a6b3114e12e1d7f44b0be3bddff06e1e9f2dbb1aad13f68a5" },
                { "pa-IN", "3245668ea742e7301829ccc6bfce4260f03b8c412b170ebb3575f644457fcf6972399b8d5c984a83d7e5cbfc5dfbceb97429449ed4dd3d7445db4c199aa50757" },
                { "pl", "2a31665267fb1a331179b83073e54352037f4673d6751f62318bfffa29b491e6d5c0bead085f3fb0fa8a851dd58533bce25f85f14083f42844434213f0cf70cf" },
                { "pt-BR", "01a47906306601fe7795bcc45dffded79f690677b042b4b2a2d1d2f1599ee60ebe7da2d50036f0c1d69c95af12a33a2263354032aeb1e2cc46d26e2b8178ed79" },
                { "pt-PT", "ab7fcefc51e8a528f9465446bee8aa8738f201ece620b593d63b85823b9b6bd01d5e91cf4e85bdfb796571f4be7c9b757699737f820005a9dd0c761ad6fb9410" },
                { "rm", "047e59d8ccc552a897bf148842baf7b7ee178fbb1cf89df9c3b138652cae79f690b8a464e66206d78ed7f25dbfb73cfcc848e39636da11883d7c75863fa0842b" },
                { "ro", "8592cc2fc7346face48218c51de273921020a800f405e3974e52ba21c3190af030b4135f2794ba2bacdbf487d582d007858f074a54a2a56ac5c90364f526dd18" },
                { "ru", "baa6f60bab9cbe7d6b37766ea08bb7be40229f4d1198c1cb67ea7cbda9692e26c9a61a5f3badf794ac1ff47a66a596cfcc99f54a5072a2f2938954e91bf9fa53" },
                { "sk", "1d3d449f0bc45dacd7c3175a5a8ce177cdd2e89cbcecbc4967320084248854bf60b1e3e2997decfe7c3c330fde7448fc7168ce8f3ccec0fdd9ed7f9fe9a4ba30" },
                { "sl", "5d148cacbfb45d4ac713e3f626a0d1f5ea98bfc025a62e6f6a5bf86a1c9fc63e496aa0bb473029c806dc27a71c5a1d53f32931660781a63f69273a5c3a49e4d9" },
                { "sq", "162c81312af358c0a3d3bc99aab72f1b1d4d28cbc858dde75d842d7d6f069a5d40e0f167f867cd228b17b59d2ed92aafc5f3164b2673125101a3f2e4f673b14a" },
                { "sr", "2f60195e70fe1622328ee7eabb38a70183cdead5e512b48ef2450145dcfdd3b8ffe8bf29d149b1415659de80a47b30b9dd88deb23d73a3ded2eccdb4ed11a3d5" },
                { "sv-SE", "f1cebb8cd2d933951591efdeac563f8323275202560ef65b47b87463140c89ab274414f6367c124d474be973264ab00e5fe85092b3660872b6f43a6bfaf660e0" },
                { "th", "8ea27d6cfac851a267bfe4d494c85006fd1386137c24ef0f99ce3994a6dd0f48f1a39f658939725cda6c80ace5c8d4e260de0dd24bccc7ff55c3ea2ff0a4a09f" },
                { "tr", "74a9d88439c7b0289dd16b7bc16b9f899ac25a2d0f36c4c38e7031d269aaebc74b988465a19a6377ae63d170ce72b8e99135462b68ae89a2146f903678341f90" },
                { "uk", "f24b1f781f124314d6dde00adaf148eef33f39d50aa394014e5fb6bd89a12f87770792b9d30996821fdeecbabef609463a1524dfb127a1c15e3c5f035f2a6de2" },
                { "uz", "603bca4d10e11427e0e827ff418e965496f1303fcba6868980b10fa27ac2832fe767bd4e42576ff127eb9c4d1f92091976952e1601ba6e12178490332d9cc939" },
                { "vi", "f6b43f60beb73455e49f2e5d561a54bcf9440f896851977fe1c97c9cabbb365924250c480fd722c1408daa681c12144d142c8d044a835250a1479237aab38e13" },
                { "zh-CN", "c84bc1d016cab20e89d8ec34d5682893ea94b4f8004fcf16af1dba135bc9fb7f3014a0babb93899d8c16d2eed0e96350cbc21ff1fe27581c24f9ed1b3d40e435" },
                { "zh-TW", "c7b103a4925e44616688c26342593222548c5386cdede8ba3cce24d3f8651e7daf2e98886833ecd2149e0727a54add7898d57dc17acccc41556ab17ffaeb691c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.5.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "6de37d08a420674bd34bdad99199520517ad2d4e4f210d2586b2f2eaa49bb4b5ea679511fdc57f2eece2471269d5a061377b0a4c0f0227354805c2327cd5a415" },
                { "ar", "0b1172bfa3575a68e5708307c374f7d50eceb6973195ff339c6c6570b10eae0f5c22b8f6884adbf729d4444b99ad4549d92fb3dd80c96e2a7dcd291b6c77a128" },
                { "ast", "12b74bb398cbd1963e0cc19f44281b6d6a48f8585cd1187016986c44d71f06b4903235ff2b0da21bdb00749e40c88116af05e4a876cb4d79e7b6bebdace9dc48" },
                { "be", "38fc80152cd9dff62d453e68bb08e4ef46b960508cc63613fa8ac5185a639bf704799c5fff40e33dc7bcdf35d77beb307006269cbcbf687bbd47939664b2a38c" },
                { "bg", "65476214a257e0db6af451ed2807b688edc35c272137ee76ae61ee058fffaf6734d8b83b385e7b6134978684ea2a4266243506c1b8ee913dbba4ff4ea8f10917" },
                { "br", "a668c2c98164167e96445dfa43dbcd8f070645d9b1a737064016010f67ea212974900efdd118516c7517956ecbe631b22728b4fe400d85f13f27d8952742b755" },
                { "ca", "31b54c6f1b9c9de7dd341ea7a560b356ec34de0a1ef6b35979cb8915f9e6f04f1423dee1bd75859c7ebb4f8c90617cf027c912742bda78c38086554ec5bd2b8d" },
                { "cak", "bd5b5546ff6f6642ed57e5e14fbeb186f8c1e60a0affe38e8e629240ca398658f55489b435abd58149d4466398fdca0dcb69dd88905b19f6439d0bc588382016" },
                { "cs", "463fabb429b53d2dfd0593adec79074776a3035fd4d90f8799e2b3ab93d434e4a6017dce4bd86e316d87cb9a7fed03c5286777bb940768e7ac933c73414b0191" },
                { "cy", "fcd49bfe201dfd334e283f71a0d58df8cf6748d6ec10b3a419be71c96693a3aa87e8701d88c3db9503092352281497471e5c06067c3ea8811d185ddeca23f128" },
                { "da", "b8e71b8a24d8fbe143a872ea1b22db826a87771ab7ecae619b512c56c91af511fa336f2eddf8d105088c19bcfde982fa9558dbb55977d00ffad19a1b6893111f" },
                { "de", "1d3f3338b9167c46500c88083f4d2e27800ac111c9b7da02ca997104ec2a45d4f6063937f48c0dc0b499a54ca51465e5f79404d04c55636c94a2ffa64075ce55" },
                { "dsb", "99b6b3f5c476481ec5bbfd633a4da9e7fb34066451152b0e94d874af22a52ab569e369298e0d3cb8bc8e18cb2687666248c13612daa591d9cf70bc2d8ce1a071" },
                { "el", "424bbf559d610f82929b368ad735e2321b88993ec0857bcefaef5298d9712b480dce6a63161d81cad0be1d8fcdd26545c82c4b49315e1fa353b1570a3466bca3" },
                { "en-CA", "8706008ec36adea7ad291cc4811ebf823adf2cc583de2a35e8549c60ceefde7f0c07052b1704cb3b2a16d39a4927712bf9130314b7e538bb8a8ab7009910e149" },
                { "en-GB", "0b9bd4aea29cc9a2f95afb9167fac7e8ad0b701b73e1917ae4df9e22a26224bce371a2d373c6a6f0754699c9f38bd53a27a7a2787c7f261da145ed73ed83a2bc" },
                { "en-US", "7192e9240263fa21ed9f1fe10b2b3d86c652c8766db026e42dc1f03915f51cbb87b70c4ad57b575ca5aca7da7fdbf2c5155372acc3b0bbcffc4fa3277f166756" },
                { "es-AR", "d9ba37be13e879ece03bb220dc12db234751ed341eecc1512de9f7e5a7df73edd70ac9f95f7b746270cc1b4c685699957e0fa8515964bf7d2694019f6d451bb8" },
                { "es-ES", "a154c982d5bd5cd27c42585c0792565f2efc3acdc37689faf250ea3d6efd776f60dc6afea62b16172228b30decaa5bd535f14d4e9018999566172b6495f8f015" },
                { "es-MX", "00b64e6c529928e5166ee4d026de7c71d693b44e152d5e66dac7b9a12b0cdc9c0ef986eab142b5bed4817930c242260204ff03055204077e19b24dfb26ce53ac" },
                { "et", "a95cb167e082b804586d644b44853be9f1971fdac60a6b5cbe174e0439c7436b40b5bf4bbe95ae1367d3f4c70fbcd85295a57a46e1b19293035ee01749efbaf4" },
                { "eu", "be2981c3045bfb8725c00a7ed483449884d3b9d6f756a5c3408d4a4705eb8b5d34d49525cda33fa40426dadc28ebfbd68565c674fe5cd5f6ab988ac4285d2ec2" },
                { "fi", "7206f4ffbe49ca9463e4592bd5bc493325ad104a5baf4e3bc39348b5a0828b8795ec60573a273db2043eb9b558612931fa44b3a2df3587ccee9800f4aacb881f" },
                { "fr", "09d0e1dcd5589f959e0e420ca34d707309562474e5b658c5f8a0915291565418ffa6fdf919283e383fe9eb7bf542f84db56042e83a2c388f65652711ad69c0e2" },
                { "fy-NL", "8ac9d51e973bcbe4761a7565ff1bede04bb51a4dfb661e12a25a472cd16262b84da2cb811fca1d364edb7b0a3f51a5c7434b9bd61633f9dcfe31b386123429d3" },
                { "ga-IE", "4bb1a1c8bb895344b3928bc7b017187965d33c231661ea075b8fd5d2e8f04a9aa5e63847d4a521738cd31a9bbf07db276d46fa4af7175af6f110bf09a7448440" },
                { "gd", "5c412f6005e3c7380561cfde68c5c135eee123cd9736036f909b1d66f365f612a1b3489fbba622f551937535c2527080e7ee0d8343c474e3c197d819164a8ec3" },
                { "gl", "de16589abd15b683cf200b2b54b2fe532f92cc99c04ebec809ee990beac36d817e18b1caafca3ae00f015bcaa293b02c6b40eca9d84f7569463e970c8d767388" },
                { "he", "f4390ef8a66d0aaea1f0be7badbb47b1972171673aeaa886d1d5b2e2c71faf0967847d9d508676f3e71f6e5bfa7e8542d8e763ea50b2fe3aedaade5ad98e86d4" },
                { "hr", "4b476ea3b5e63868fbbffadd6cfd5aa6b3c3d2333bfd7f446733880e68c76de23a77dc9ea869cb4245d39fd7a70bef08022e637ed1a0c39a13f088fdce0d5846" },
                { "hsb", "3fa69b132ee7b7a5cec8deafec4c18b0e2b0cf0653be0f24ac8a5a6017e633f0efe69eab6a64bb18ac7a30429aea26083249d9bd4f6468b5864150a68d23ee1c" },
                { "hu", "2c06fba1c8b749977f648a48dfce99b432870ca65e5cae2a7e846e773f4d10d644136abe0eada7b42c070a8542d4faaeb8afc77839cd20acf742aa45a5e5bbab" },
                { "hy-AM", "29de20585038f3c853cfd7bca2e07793c423ad8f10ea788a51df32b0ad7c0942f9997e6e62de83a06d9dc28214ace9091e241edfb504078c4b6d1c3513606624" },
                { "id", "d3fdbc44c8c1816025cb0fe6e71b838cc951acb5fea3dc6b0ac12275a7fc07a2d7dfb96d6c20c4d59bf62f11ac8d57c43ade79856263e296f288e543acad1b85" },
                { "is", "9721db1df06f23e33ee8a915d4996783cb527ff70644e9412b5467fb08c6c95205b1c190a0bf057671669f540765446f88c591890c750eff795c51215ccf3e0d" },
                { "it", "56f0d80478269051350f078c0befa9160bbc6e43a570086b6cb1da319e0c6aef64aa8adaee6497e1f07797f140f80f89c636767cc747129d9e0dd54d2871241c" },
                { "ja", "a7d38a11a9348844f9ca5a9269833afe17c682ae090985a2282248bab2e88b5442a1b08ca469a0a1501b73cb3991299b0c74505c035867396812af30dc8aaa36" },
                { "ka", "6d71f86be167ccedb5e4f023b01d14d773fc0dd0e55b5a56cb55e9c453a8126b31ff422d295e12107d0fd0fc0c60c512b2f686aa2f847903c8108250ccd02de5" },
                { "kab", "d9f747db178b94c7328c8447e6e20e9aeca3a6ee526400548f3ef52cc740f57b681189bd8815878192858f7a8f3fa75fc657748fbdba4c5afb40aef9badce8c7" },
                { "kk", "22b0dc5241995b06e1a8099737f20b61be345c1d2dd116034568a12bcf74ade78fc0413b06d697c2131118a596a30ae0067238800bf3b8a6383ec9ce0ef758bc" },
                { "ko", "0b4b04bc08bf5cbcc9fa44a5e55cb0815aea19b15a3be41d5f96d7d63adab2dc700461c67227a045f3a46bec1f58384e8b791906cab906bea12c7d791c8b1007" },
                { "lt", "bb0759e68dbe11166ab073b707e0d9f55cf1dda912499129e18062c1ee1d3ff099ebfebf098805fa9f47f4140025044322c3b46f4e47ec8a51484794e6145b20" },
                { "lv", "de26b3cd2158cd0b3815b5035ca110478dd04ccb7e0a15e60b749c426ede529a9fb4bfb0a42728d5cd635bfb3203b84022286bdeacc29b791622534d2022de5b" },
                { "ms", "0a024e5a318c61dc3ead8f3d8768fed3e3c96d78961bcbd24c93680e1ad1182121558fc0a6d69052e9515c0e77b87c38caeb872727c95661ba5e22dcc1dc24e2" },
                { "nb-NO", "b598f463fb05f76e0c8ca5342138046949f9ef6f0d9986c351e05f54a359240b52319275058996c720be91f12bd50565162603ca9db5515ab297eab82630ffd0" },
                { "nl", "b764392ff6eb86d57fea13d4afd9c8bdd83c26bed05729bcbb8b5fd91dea8c063fa16cdd1a9ada2c681eca9c08f9abc09e72272315697a4c5f9d6c5f92db55f3" },
                { "nn-NO", "dfe89ad833ec0271d088e6f50bfd35c771d476274bf66680478d99a7f1d29c05377a219a5e2f7597b12135fa9bf21be02c8ac0853fb6122aaa5a1fc42f74cd46" },
                { "pa-IN", "074880671e65c39083cb98dabb0e2ab8ec33a3aa8f7d76d16356b73184d6e302d629774af0ba621a903084dba2487838143e4b4b1d8270b7a649d009bbeccebf" },
                { "pl", "0f118bdbb18ab5f6d3e5ccbc163046cb387ef773e779e3f629f62712e362eda29c8c712daed6ab9185a958d9522cf496803321d3d40321c1d4526ee62220b2c5" },
                { "pt-BR", "8f419ff66fc87ce69053c238b415033e7147ac5e58f3c34432e3dfa45d9730ded74be0297845004bd8f416e0881d345376ddbf70e900833559483524f76f9a8b" },
                { "pt-PT", "273c1bc728fbf6b0f0e7256700f89df3eba26b24f17ff8b66525dad09cafec7aa236afa853fc196321a277e3601d6ae65c8550fc271b077975b1c54eeddc9a18" },
                { "rm", "11fe34dd1b19771147c1652887254b96a90416570ec3ae55d51f929c8a2661b3e1961f3e1a2e33191fee4f4defdf98900c26dfefd62536f16f10a0edcadd7315" },
                { "ro", "e47e6fbf0bcc8ed892450c74be6658e999b4601fab5c547a8f16245ac50adece34e29bf5ce2c34b542a3cf5989b58622368a8021143605947bc256d80ff1fb07" },
                { "ru", "471f816f03f45559df15794f8ac8aebfdcdd937e38ec7fc1fb4d4bd27f43c17976c22fda2e54e68a83901668849e8c2a438a9559634c3b5822926c7898ab378c" },
                { "sk", "c47434bc92355812e4211986e1c988f3c020883cce8b842bca1845f99e896c8961060adfcef98e0e77f87d1134185ebeb99729abd00bda198cf72d0560dea98e" },
                { "sl", "8462a6e23a2aa7a3d62ede74498ef1eaa385f157fdafe85365d7000089e991100599e4b12435dfd117d0028e18a64e197f10b14da0d81f2dca61579e9a85ecfe" },
                { "sq", "cf36b7f3686f260b2ab68a178d5d7cfd2b883353f80904e5855f5ddeb11c53006ad8be7ebb4cf7efba442851dfeef8bab1851075d54d1b2d86b601ce8f883f61" },
                { "sr", "066895fcd79a68c0c15581543a952b64624dad09d086b415c2efa3a885819637c5892544331a39cc2d0b9827533f77d4719bf1e85ba7af2545086de731e55cc0" },
                { "sv-SE", "e80791c5f17ff7c4e9e827e7d7660ed32d8216887bb1b13ba076a44da93fa316ba0433a33d92d7da7de809695bc4fe30b9ce1b6fa5047b7b506b6cf2a0ce75ae" },
                { "th", "0793fc0c5aee55fae8f2c2b701a0b30140e3b4130be1bcefdcc813f9f9a90005036051188f96e729e14b84f963fe99367ee4dc2ff009685da5c6dac7878682aa" },
                { "tr", "47e2463832bba0c76bb23397d853571524d697c77f59234b8e6b3c1289c3342be0b8fe828f22a108db3c7d76e532e8c671a133f9f5e9a2c9c517ad71c3687ddd" },
                { "uk", "89800b9fbfc51739ba40dc00399af594ae39c8d99ee28903da605294ff9b709d4fdb0ab9a3111c4366d09c40a9e1a2b70814e579fd186c589585e3e2f8f41d75" },
                { "uz", "0c3c54642bd109752803ffe91ce9fea2c395c1d55970f40f754724bd6ab625941f3e0f2ea53673d30e85f80b83064ab5a9abafa6d80581bdc8647c449f516674" },
                { "vi", "b347d6a4609ae12cea536de908fc3e8c9661b7c14ad27fc41cf11d3548a0c9ff01ae2c226bf1fbfa4d9620a2bb5737dda7b17d537ef5237665f74bfc7da91962" },
                { "zh-CN", "8d43fac41bda8a148bb04c7b64dce48f294dfe746fd2640dd83a31fd94475b226c1855242952bd7e871ec6b5e1247afa7fcf296d10ad5a3524a8dd86facc85bb" },
                { "zh-TW", "277e179ed459afebd73600b5f3db9c077f2427db0ddc8fe49479cf6ba7d02c7e89d1e52673a894b4fde2ed81137e9177fbe6f5c861020c3d5058e1b293af447d" }
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
            const string version = "102.5.0";
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
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
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
