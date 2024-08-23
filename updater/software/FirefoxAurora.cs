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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "130.0b9";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "7a2f209feb4bb4752bb45245873e6da9dedd9d555218b79e44c130e044d0322400c693f30b69ecd43d3aea77ac53723df54fb55709569ecb68ed7e74152d39af" },
                { "af", "62c3ba5f87161217d6a9b00685c916dbc94677507265faf876521d836d875a4c0b2d979641dd676f86b1323343c3754c3c624e44cc12120378c6c340605f93b2" },
                { "an", "f9a2c5e3cbadd113a6cb10e544cc8dd130c810470cdfbd7325e8d2726b2198e33fda6ea95e0bf987783bdcae983526eb8a7c00eb8761f313e8a622803938b312" },
                { "ar", "7d7bf1401e3c7648d3c012c726cfefb3d3d447188875c8dbe4a8dc397873a5557759042740f19eb8028af4cd6bec78f9ab08910a27952f8b44c94c9b38622d95" },
                { "ast", "1f4a1f3160c661d7eefd1541ca38469b4b16fbb366b9fa64f58160ce25f842e2e520c5a615189ae9b8863bf7c5b4b9d7a0ed2ee13ee956c73e80bc72f3494e9e" },
                { "az", "85950f4f829330627e6647d2e6d878765928c3637745ec7ad0d9535b5cc977f025745b612f31c87631dcb64f2ade7ab2fb10e9e0b1c4b4374c7fae62a6ef7713" },
                { "be", "be2a2c7eee1777b74887dd9baf7adea8d8a7653a155289b65e479528df96218c4c19a6c71abe286e6d60ff2a1c4cc94eaa7a6e411942e5e37574306525bcafec" },
                { "bg", "0fba3802a057485ea141dae2c59279149d746c43e4278380287b8ef1664367b219f90a8f95239a1ef134de61ad83b6522757c43f93730346b6dbcea43c6b9907" },
                { "bn", "351c04e81f565fcf505028e662c2e7e8b11b9f7ffcbec454011a47f51cd7d03f53bc3da0f6d38e72b0f6cda5c257cbad4988c1efb51844e89159f872151032dd" },
                { "br", "9f5941fa4f657a071fc7187885750f632b237a83710d134a9990158e33f35d6b55cbe7b542ed4c2326f76093dc3a3d6b644560a6b4be5c17ab33f0cf353d2e2f" },
                { "bs", "1a93dbbf04f623e91065b0f14bcbe38283147998ea49136683aeca3b36020152bf8fe3e9ecc6e99ce629a08dbe05d3d6c2fd540a113301d72832ccf76c866b5b" },
                { "ca", "5e453c1eb1dc8e078e22ff50835b47ab52994a2baa8faf35c808c870be7c37d9f9503d2c41db24f952632208bc340419c7d22b17adea2a7f5db11aae46d1e7d8" },
                { "cak", "64c02c9d792ef6fd155654d38877c7d7d5df1eecd8a7c4bee671331b36c4388ccde3e1a09c4c575add2990a4e80eee8c7eb8d8304daa3c797d0d3226c39b2d9f" },
                { "cs", "91a2e6390941217d53fa2c4371d5e27b4c4db50031867cfc67b99a960cd12f894e9d2b080e1931bb2c2b9d6355d6d501cb193e32f94d712e4d5abd48c5e23260" },
                { "cy", "ec2f19a8a7ac0ef2fde38f3e1e0e7029e1673452db21f85903c82e2c87490225ba87508d00b1dc61feb480e9db523c3b405cfc3e7e0d791764296c1aef47e85c" },
                { "da", "8dac2b925cfb624296deabe9b4e4952afbd7ed339cae24ad3a99be4136aa1298a4a79ab13b1c3eb848e852d636c9ca109345794367c4ab4f621813f8906923ae" },
                { "de", "577483238fee78ae9b298c2ff72adae6544cc444e21935317b88a62bff60039395e11ef86a01f7277fa3a4f443975ea2b3389e069893317d0241942a4965f0eb" },
                { "dsb", "ddbd0d0bfc77201ed881479f95b64f85dde0e3100c2a5fde4beb59b98c67dbfcab8324be072de3035df973710aad5ef6be365a78df3ff0bfbf985fe9f375ac5c" },
                { "el", "602e8d53c5162e25ae8f449aa8f3f383c99efb396d183b3bd283c6d30d1d3c950370fbef42ea8e2fbc78c7fbb7f27703ad39846613bb72ec6b5e5f423f976a1a" },
                { "en-CA", "5817fd9b770a818841714985b4abe0ca8e3fd62bc8cba7ee68faa58ee9c91899bb68217e252def219f99b24dd1717d7b26fe1afb0e74f989fac5cfec15bac3d9" },
                { "en-GB", "773ce2bbd5691a161646fb825ff991d9899c79eff8683ad4f489c900d0be141158fa68a52386e29670577bacb88952adf56d470d99a36fc80dad8be23fe87c3b" },
                { "en-US", "d12e717ac044e90e1b49fbe0087fefa5d55b7828a106a1b9702118246636b68aed702c04e8084bc24c4c96e366e34dfcb337c38d06660be6289e5e61ca2cb9da" },
                { "eo", "7cd8655b114e7c6b8ea067dd399f6fccd2134bb77f07c02b76aa7d11a78c6b32d89291783fa57bc770d3732e1e3f349bedeaf5359e3e037cf81df2f30fcd7f71" },
                { "es-AR", "b839dc4b8d8ef30b5f7dfcd292c2e1c12431b79bcb308c9f74ba7658096631a690a3982d946d22513a6e38efafc67ed920c9e73518b2c0f2f42a3e481377c13b" },
                { "es-CL", "b70b0293fc0b1cf3cc6ce05bebc01a5622a51c3394de348cd1a82814b919bea9892f20ae48f87a059adfffa41ebfb542b77d3afbb5e5b2c205193ec4d0059e79" },
                { "es-ES", "340ac7015624c92391cff38f6572fd44b17e7d4939321af13812b10480df53b65d325f80773430e80a612f6cc7048b67cf7ed062658738745ea8aaeda4adc346" },
                { "es-MX", "dd74e7b176028eb5ccb533fca8bfc33d11bfd9b5c923657ea604722308cdc7e670b9a472aa9fa8c5b0b741e1aad0e0e63f85ff27c0ab47a9a551a93c6c5d634b" },
                { "et", "39a537e87f74e950f9119fd3d6ab19f35d336fbd02cbce3d8dd9a44dcafb6dda94a0bb0319dfe06e1a6f0047ef80a4c0475f56c33967800879d49b5ceba9dcc0" },
                { "eu", "0c7bff8a70b0a197bc3e6a7e7e42a9a9212241e58f8daac6d8b4ff7bf3c4fab96b3f252a843dedba5030c37983ae1cb9cb2b1dd44360501915429c68dcf1e41c" },
                { "fa", "5c05907d98b15dcb6b8fa19e6c999a3fc9dd61dfcbb037b4c7032e9da4abfa542cbc6105bf173aaf708f906cb01689cf49794049bde414bf29b7dbdbe1154a5f" },
                { "ff", "1ff7dde6ff2eb29230bb21993ef856d3a17887baaba81e480fb0531986297c78f427f62f218c55718df215f98c4a9f942580721f13467659418eba539db83a17" },
                { "fi", "d7d246d440552abab728c925e33098a5db62948dded0f198057f3af55594ccfbb1666a8d3672237e50aaa7f180b16f557b67cff92d7b04148504139e7a966a4e" },
                { "fr", "63bd5b00ddcfa30a5c240e08c98d2669a08a692105a22a36402f001b8cd8ba1d54d1b6081f9b8400c8dbf5131a893c6e5e8441da90292d53e344209f38857a3c" },
                { "fur", "c3bdfdd373f3a80c1ddbc2f45eaff400c1fbc45af2ead5768b68bd78dd7343b8fd4f3ea61b14547962a26154c9b4cb11d6c6432db148d8bd2d6c52d44f17ce46" },
                { "fy-NL", "49cb170167d8561e4127b25c1387728cc5a3711e0c2d43a5aa7a381ca50c75356082846149a55597a4be508b01996096c10e6dd33544a9b07d8fabf38e7f1f00" },
                { "ga-IE", "7eb142921f774b0dae6578b8a31d5695f92c9f2c53ba5c7d91c333228c2fd189364cb2878939a3748ca781ff24ed169bb0ce569aba6d556d0b7b521b0bdd11e5" },
                { "gd", "1cb84d05e72a0037552c9e036166e3b73e676f3223b86b93269b9539069ca6820834d02186d0d99d0f40361d738cf13eac5286d704be073c8af3ed7b0aae8ef0" },
                { "gl", "9078c53e4e159903fea52d0950145f779511915986fd11ca864c7196d45e56fb129d542717253d8317f2ea2b15856e544c59bf7d3722da573b255e20bcd6b2d4" },
                { "gn", "0175739b1e8833e08e030a979c7ef8f11f32d1e042cd8a52e1bdf442ffd0fcb6451ece9f26baaff7ee1dafcc8573a1cd4af89e73cabb925cbbdc2b4b1d06aa17" },
                { "gu-IN", "8c254aee4cf8701bfd9f4782dfb9d123cbc56f4975597955e4c640d0647429e56b0e938f81837ffd9ed9879848a739f54b75f8c534ef903fc64f6910bc6de9f4" },
                { "he", "ab5e526bec1aa8d6fe4b98969f75ed89ac0b428bd8fbeea4a48d4240dd09395ed74fb44f43cd1906ceb148ebceaf849a87ed564b9ecda8ebeecfe34e0636cdd0" },
                { "hi-IN", "f244401b26c0eeffa27835c8b04c9309caa7eb899e72e07291e60ef502ed7d98175f2ca30ce8195350d489dda24f65a1d6497ca47836c3f645e21d6b6ba75d99" },
                { "hr", "5eef78ebc1cd95884a331dc8ded4d3a6de2e385d391cdfb72001350c3d0eb61375072028a4e7edf4035a78fec595091d1a0f80f1c9a1b2e4cb56254bf0f79abf" },
                { "hsb", "282b3848e368f974358c7c80d1cb3ffdcbdd2392b7a862a2e372d2249713e5a0810fa4910984b5d8bb8d61f5255d0d7ccf5c301f35281bb3e6cd0dc51189efbf" },
                { "hu", "f9865e0a4856bcb661abce30290a2cf23d66f88dc389e4b5ed3cb28740831c4a25ce99d376aeb017e46c6852b389ef6d4aaadc674c3829d1f31cc7bf2ca81d29" },
                { "hy-AM", "2b640a90b2889a51dc26e324b531a7e5159b2bdd2d4d96e72f620677546442022b8dcb7a04827603a15eb814dda627916bf169f3a6288fd40ea566eb270dfaff" },
                { "ia", "7ffb80900363eae8837c31946e80b2017f54d2d03ddea19a13c1eeb05018ecc7f3733ff857ed03d80fa522047b872e2c61f7c4b72222550ad8fbdda65e94db27" },
                { "id", "6f6a394330cf90b048f41d861f1df3c4e6c836964a31b31fc0fa9c8c7104ddfa6d56cf8c77c54ed32d0674949e2d6dbf2f7166633f943b647acf11cc1b2ab1a5" },
                { "is", "5a652d918629586ee382e84eb6f3c834935c66a9ef340a83ae5be97d8f9828767b98acd8afdd18302b738ac52a2e2954ae972f6c3778cebfeeae33acb14d276e" },
                { "it", "a35d4bab74627eacbce9eecaa8cb5541c1314af8489e4f9ff0ee723523a08ecd8d5e24118bf7ff0cf3dd9cdf5df035542c2b413622f5c3fe313b5ffb49710149" },
                { "ja", "a042b358635e310b7628d94a5f3beef5a2f229c01e5a7ea45b871f5ce9162ebf88f07d390eae4d3283fe3ec31c6ee0d67136e599143ed38b37b5eec51a8e5818" },
                { "ka", "d5337707ab4dfc5cfc561572623cb5b8dc66d068ea7a057068098cb7c8c537c7c3d40384d530fa06c9bead6f13d4fbaf6a7576191c14b9ac4359cce78d96e69a" },
                { "kab", "82633ae6161afded497bf2b6ef2e219a649c093619ed91d84fefb80c0c749b97fb2019e5b5f680e9ebea7a40aab597b2b2240d76ea3c77c44c70826f652f125d" },
                { "kk", "d031aa5f8c958250225eb8e470a4c146e4b70f1d9749622896633e8d99bc25221371efdebae1aed73aa8fa8d5ebed398db84541c5276b793391932e3ea60f2b8" },
                { "km", "e645d3d81d12f6c9fd1b0f4e9395993a100bf646984f4db4e661d3504ed50aaf212dafbe92d416ae1000cd0f8395e80ab7344c248d0c2a63230bd15c4c49a2b1" },
                { "kn", "7d0677f3245448be1f4f41784fde01aa47ddc132f5c66f7987c4fc63ad2a739008bb7625c21af80fd4aaf06ebd4e5dfbfd42f0fca427097934c33b6d074abd14" },
                { "ko", "a3c122e9b2b68faa0732760c716ad1121bebb87eddb43ed2968eccc9880a4893dc5a5698ccc83c8a48c74dcabbeadbb7001ad185c5ff7cfecf488ef054734b1d" },
                { "lij", "c73d5b8687c170c54a8a7550df110b5a73b8b257e917e95494d30b4be8f1712e266bbe00e7bb75fa1536d0c4ed46912eed10fb7fc6a8dde106707e9ba738940f" },
                { "lt", "5d1204b6b845f3675f1f05959be1714b51ff48363b715fa0b114146720c4960c7a080f21b00a69606a4d005cbbef57e853f1d7203766bfea6149d235a28adaf4" },
                { "lv", "fa73acbd9f7d3ad432e064a13c12ffb68db8acb0a5e07dce59b30ffbb7c71dc40a52f2eb79b11e79de6c0cceeef28bc0c51d8884d429d620eb9aaded913b04c4" },
                { "mk", "a55ab46d96e7320cd5b999541301f22ef5f00d94665c48a3eae9174f6875dc046d1a0525a4f2b8fe120d8f83ce3304e571e3df939f58c3e5e660d35747598649" },
                { "mr", "d54f471374e10753313334b2bae0968b09c5ad47d8e62a76f4cda6dd2355b2d1e2a3140df141839ab571621088d5a65fe4defff4779a2abcaa01c94162450786" },
                { "ms", "a72896c36564e665039e2f852297ba5c4a994527dd0c8e918de186feddaf3c0736050e63049f0fd783a9561115559f441dbc61b5b82b82efc9ddad8acf387bc6" },
                { "my", "c0f86c3a051db95c1e73bfd1cee9c4e74ebed96a5d36da439aa65b07c79b6cae41440265529185518091f21cf04d2ec6f62ca29314bce7d73b776711915b8c63" },
                { "nb-NO", "284c368a2ad28981172ddb978edefbd2734763dbf7039522149dbfc0fe7810d80482f966d184f2c06c4e6da23c37f71f032f193ebd30c9ed27237537d7d42761" },
                { "ne-NP", "349420f12cfaf78470f270572af088ece0cec033c6b46480761a4df36e76d11c1e888b4d6c2d2ee522f8134c915f64b927da6886b929125edac032fa35915b4d" },
                { "nl", "e9a2555696301c8514399bea3587ff703b3fefc6590fbf761de2e193f4d7143039fc6749b72e9441e2e830e9bd169dd34f55e895ae2642b4d3e8d558c9bbcf62" },
                { "nn-NO", "00b48521305b226acfdab352a19b9511137dc036308365859456b60a0048351a078e0d4c2566d397a596e4c14b0edffadf582a0dc87285122023642a76ef9403" },
                { "oc", "1e197e94a07a97011d4b33e9c230925ce34ef037b4ebace92ddc0a733ef121a300832fb146d35ae8beaf39f6ea6f2727f0a96d5842672b934eb3df4bb122d63d" },
                { "pa-IN", "6f52dd2040b8574440197a92ccc4f7221c5a1f15112e1e6a6b99355a25d0d20ca607ea819afc021b90674f0f79005ca7df1c8265b7b9852edf0b407b3209c518" },
                { "pl", "375f4515c76a1b287632c011630de150abf9789ca66152bad00c159c8cac2c1e3e3a542bf8ed55f0fcf3eb89bcf4e84c61a61d9bd8d369200ad1fd5985ac9638" },
                { "pt-BR", "a342ee07e437e6eb5380b89fe35d2eb4de86e774e594d25216a8da22549d0973a34af6c83aec6270066ce57bc0cee76fb883ab0ca443b0edddc15a53bd2b5a72" },
                { "pt-PT", "51307466c16899d3a457769c1f6ec62dec0805e8fd0928b9ebf26bbbee12c15d3cbe794151fe710aa7af6010d8c09b3e039be8a29720686ce2c19b5fd544d6aa" },
                { "rm", "e1c1b7fac589c0767b9c57b20c7f1135b290414390bb4a98ac5bd610168c6372d8b80916bd61c07d089a8b65ed39661593c6faf54a7525ea4bf176e3679e8091" },
                { "ro", "77dd8602110f612ea78c2ac918022b9d230c6e94529d3f7db42adf5d9549fb386c48adf88b004399731d48a22ccd7acde718e5526dbcc55af0800d26fe551477" },
                { "ru", "ba5e6c4c9da8577714c3c61cee260c5cba37798e3ffa78ec772d1d01dd748142517a854863fb81759370e2ed1f47ededc64d4b42760421ea985edea3d227cec4" },
                { "sat", "fd4205c021bf58c7bd90333a31dcb3253f05a261940fe5e70d044a4bb4bde2d8cc43e84c685b147dc63712b0b5c41c6313f55384526faa0bce86513e8a868845" },
                { "sc", "bbb107b4e80c3efc6fa435104014af16d4675a725bc1c378f49d98840ca72234624c46b1daee99d6225e60be1a42286208050e7c3a4a2fa251467ddbd1c211cb" },
                { "sco", "64c70134b3026d5e5e1913b57464abc13e0d7047b37ff981ee49594e06b69a5ee0a30eaece6c93729f99a0bbb073ecd1c34e2e7201616a3d1d4b201f4dc8c7fb" },
                { "si", "beb68a142b381b73f30c76e2207a2aacf522ceda48e0b29ad9860d864a0f798520353b8ddbee654d7f3a31fe4bcdb6a6ee49a5b6aa143bfa6feefcc10def35e1" },
                { "sk", "a9d722b229ecb7d9fe2d6bbcfbb1f7c88e4662e71b346568ba5290e4a980aefdd4dd605f204fc82d038f5e1fcf1238e132b136ba442c09fbb8faae5a230d1a21" },
                { "skr", "7b3bf53fd3b6c00e5a622d845c0f44daf550cb709adb7bc5fd8cd0f303dca657529cd2d05f3789c2c34e10d0df21e6997b95d9711e18e2d11ea017e2f723741f" },
                { "sl", "feca7219cb28920ab4485363572901febf79456c652a52538158504f44a412084acf34d5cf3f488bb0ee4d54daf02dd43e3c196f757af86a5c8a626e8956d68d" },
                { "son", "ec8db655fb406e5c40562a1a327fa411d0414a37b860472954bda6d306929cee19fef9dd603718849820930a0aec34bf29af30a41dd66606cc1b2f2734871026" },
                { "sq", "9de1af4d1071c3b1a5227c51dce843797ddf031e9aaa13b0aab6506d401034b480e6aa7fb6c3ecf9521d35c411783bc6c829a841b40eb4d44250f58e7de760be" },
                { "sr", "73d096a577ba67b50cad0ea56129746260d3479e63c665bded23acad194768de440da880478b7b455d3aeaa03e86b0b70409e8a0cbf8c5da81bc571ee8e6fed1" },
                { "sv-SE", "9112eba81e47778a5c0bfd467dbd1b516998978cc19b5bd394fe66c74604d0c98c90bed7b6ae0c086db2b4bc6eccded8264c91e595c5c08374990e0ef92c9aed" },
                { "szl", "12f0f6ee478715aad3c3448bbe5b01e9bfb1ea9ac0adbfd69471033a24ad36c88ebea5ca776b57b0fa261a6de6c8c04f7b45441e3f7101a8b1c1fd8f0831baf0" },
                { "ta", "cfd4e66d9a6c3aa7ca6d070427dc6c95db43ac9b9de565283ad084faa71d44e2eacdfe109b70d1dedfce9d4de98d53df0e6811aa9d7aef2ca07398b5ad1dbbfd" },
                { "te", "98796eff5c2108335b9d5a52f2e95a6674ae58438cce97087e6e437877a8c4694b0661b404b19ed71404ce234cf23be891ff535883dfa3f2aefd7b47a658f47f" },
                { "tg", "f95d84811cf65118ad88d9a4172a3ab5d7c7724f8b93db95704274984364dd158d23f0b499cc50f7302734eae717cd3c7f1b47a2b0f56a7ef8921b63a286bcc2" },
                { "th", "ffe4c56f6060fe2736c4efba7e5bdf2d5164cc194463f07c20a7b7270ca8eb6abdc5a55f1a8ac0e5f166a357ff93760fb7740c3237eaf37885e2edfd05c40bac" },
                { "tl", "56e2087a6479eb8a384b0cfda5dfdb26862930a61a1651ed152cb833deede092533f3b30f2d0db6ac48b3e8d2dbb0df2b4eb4027316260400a63aaa475e3227c" },
                { "tr", "64f00f2b12eba0fd3250cbaabf1e66b78fbe531da7db73dfcffecb1b0e59fc3e1b1d1f6e8515d6eba690a243d5c52b4d487f1adb35bfaf5827d55115e962c88a" },
                { "trs", "6a6d7b711eeea213529cdb780f442df8dc721ee3e4f7ccde49b42e6c134c803fb5ebf4ef307f249a3f5521e68df7ef1a52a4df0fb7f69e46715cf2dbeeaa0ee4" },
                { "uk", "57f55679003a99e1ca0ce74cd7f083db0ca42353d3f7e65b15e7463377e0bf6c3d8b271b06c930f899e86ab9f05b264606c8ce9ba7f9b14e8ee4e389588addcf" },
                { "ur", "5905745ca0e7cfaeaf55a47357e9a87699dd2bbca7f81772ea3dd40e2a28ae561d027a0f92c8935b6c7d40e745d3c2711b303e91a7cd0e8f588c856df83247ee" },
                { "uz", "2210e79bad70f4fa28cab8f577083c0091b9661c10ce6a2af67139ee217dd24e3746db354e7467574f8bc69a3c30db140968a04b9b155cc5a50de69ba92babc8" },
                { "vi", "851462a80801d458904731a002ed13ea02207fad12d052e2dcc892ed159970217edb396c6f79c34f436a66fb4b93321b25e5c63557ca4e234bb5fb8817d6dfb5" },
                { "xh", "94246d1303d26fcbe7f4ce8747cf17f6b3756f8bbb97b3bda3b7ee76e8936bc4e0ae3913e68a88a382dd9f093988d0fbb0dc9fc981c8541c7d19a4b36656495f" },
                { "zh-CN", "82f933fbc5b8905d2eda8f8cce9b0aecfb927776353ec2f5e8818ef31aa7bd006a77b860583382bb24916bb1dcf524da05c2150ee7ba35c5cd12399eb76452b8" },
                { "zh-TW", "1f1c447f3c345bf3291b2971be66184dab03313e802551cd2d0f1b6876f00ca2848acf3c6bbd187e2f10912bc68d1e07910b17a4c015f31907133d30e2228875" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "9a69ce50a10226b2ae85f0283635ec98e5bb2e9fe43edddbc11bd7ddb9eb745fcb59625e7043680954ff4ff7f9e24f154cb89ac49b4e5ae14458a0b3301f59ee" },
                { "af", "4c678a3e32405acdea0194a09c1d8696286146e7231e199db0e7e18ee244d0f9017a4452cf00a19c572ed23bd509c6535ec90fda5e65bfe50cb30a959374a0b2" },
                { "an", "e2308429ba4d9b0d95394fc2a3dd4c2f2afb31351b88986b7950f17999006e11b4f3fd7f646f4287d8254beb6903ec6f8d2157f412bf68097dfd69ec279715c2" },
                { "ar", "bd5b4f70d7ed5af8946723a2a2fe038fd7855bee2c7d2e90a9b4619aaeb42b8ee2471625d13c38a16ee956ccdc1de936a1d471034c5221c105cc994880f024c6" },
                { "ast", "9df52addc8e6a7f97850e7cce0a37e36b84fe464df397f8c8c8e2d63505ebd67d80160f3ddae6302ee342783a7e55ea847e7b55bb271116ccb2436e598f04fee" },
                { "az", "23d7374a12d94dd047fe9cc12eba0d0e698fd2471ef50a7404853cc5bd42e8ce6d6db52de1952dfcf3b5609a6524bdf754c4d9eaeb9d082c7491debc75839ac2" },
                { "be", "deeeaa4dd88536c0a33c2820431c4ce10974e676d525a0240013c34578532ccf4f81b01b37f583df771133141323bf05ba0260eb82a75fe046ee803b44e91c94" },
                { "bg", "b49fb0f58e8688fec1a45a7139cb20e79db57fb46406472b4610568959fa97e5efeed068b3e4e9cd0f3a04d1935d943fbb78d006337c9a31a6e14cb4d34548ee" },
                { "bn", "06a2858e29b48bcb3918bf53324bce964183f75374da734a851357f887f99a4677f05e02c3204970e5978b6cce0aa66afa84b705dc949e1323cc5e7ea4a602ee" },
                { "br", "88ab71c8cae0395760b8ea6c293181c1aef8bc7570d7f38aaefcab14769f6262af4747311e3f823eb5bd6f355635229e57fd2235720011be40717fcaf76ccae1" },
                { "bs", "5db2854f5623d9605b8c340c1c15b74e651eef78cfbf11598d39d017a6463208d9c3efd59d35144b6cb379dd453fa325b3f85b7021390d81d30683a3a5df0a31" },
                { "ca", "1a027efc24b3554f554e4ddc47fe5d40afa2cb64f02fcde079bb94093b270cee27f7b09bc6d8cadfd3f673a0020dd2ad5fd9ade5fb218390793158e5c0bced9b" },
                { "cak", "cc0a1f092dbd4817ec8162e17394fa914187edba26e31092d8d0342c0259e0605b22f19b8f6bcbfbc652fd6fde596dd955f5128688f4c0f3874fa09d87e3a1a1" },
                { "cs", "378a27a99fd117cc5678bcae25761b5c26ac369e080820af032c62997a05f8f9ff27bda90d1806956a478552b1886f723c4e166977e9f0cd75a70f44efdec815" },
                { "cy", "1145cec3522e2b57ab245669cbf7cef502c6679546d8710f11682acbf1a762374d7d7a84fae85eb7d8448922b15e1e470be8c78f5f6315c3f2b39621b7dd7149" },
                { "da", "57e64a0e3f413f5104a6cdf5ef13cfc44129749c18cbdefbbeb8300f139762085c3540c0d8ad7c60876c8b1dd96f4181cb7d96ae23c71f3acadcac1d5f230ce4" },
                { "de", "843b3ab05bbd69d8dce554f0cfcd50f557f6c599e60e5c07e5aa58b152ad14311e83624159a1e0fdc122cf8bb769c6c376a3df90d8b62b945f0fe60e56b70c1f" },
                { "dsb", "39ef09625c9a4f4ea98fe5175387dc4331f5cb67ba7dd2d949cb39622190c6c2ef001106b3948c115d8309a5937d69e07a7e0506573b3866338afe5b6c056bde" },
                { "el", "3aae443a42f8ad5d8c8e2f244bbab53d82e1c0f14016eee467c4e5b48e8d2c80063c475feeef16803c2bf914e4add8db8787120679f786f3e5e228561a6389b2" },
                { "en-CA", "900026ae3ea8c5357db843bf113516d00318f130cb570d8c4ab3a7829a9bc8e41594a0796e0222e61da47843aca95552d1396a618bad55f3c607fa601dcc117b" },
                { "en-GB", "59c73492705ee04c13741db1b8ae627dd7cb75aa51c00affe93980d1b7d9e9d37c04f005d518d8c06ea8c7368e61fd975f73ad7ec9346f15273b57a3bab9ee3c" },
                { "en-US", "9e92707dc287a14bc4eff6913107bcee5313a05c14e8489912858ee641c59133665588583257ec0bd36b83cf4acbe331d4797ea3be63a429f88df72bebea73dd" },
                { "eo", "097e0d27b38ff2e162e248ef83b5c091c78605521bc49d86b8162805bb5b4e51e649d5d11d163364abaab7af27530b8168299d2ec58512a163b5bde76b5ddfce" },
                { "es-AR", "0e835f04a6f9d1801317d01ac10c1d2e5ec888618fff39d4e9fdf91154d815a3dc825d96b02564052593bdd20ffb3add69c82aefb2607494ca67111f09bbc824" },
                { "es-CL", "e81aea52d35e04ccf4d3a15513c34acfe2ba21203eb3becd62a5da297e206e1cabc532c31d86a19a74ff4c1bf60c738034a2f2d6b423b3bc58c59aea1c7cbc2c" },
                { "es-ES", "bdd5c1850a1adca2e087649450f0670cddde5f63098ca928ade95fdba644f55584099378f718c024e82e91322aaa026fe89068dcde4eee1c502aca1a73dc38d1" },
                { "es-MX", "bae6fcb21af74967456ea35a7b2b289c12eb597a554e23be992d61dd9de29ed920c0c927e19f7c03ec9368f6299a12ef30d8fd3d1cc78cecbdf66d096c79d742" },
                { "et", "698cd0628d91fb966904fbe7793421a0cc528bb98c483cffc35ceb0bfff43f3852227106faec687dee028c2856435656a3051e7304179c1a7c1bf02a6f6519ce" },
                { "eu", "248a78eeb658f1c68531d340a37657079db9ddfc07475b945d782302dd340b8084f1cc76d126c0ea4d0c19aa32a927fab7a3b545e616371750cb3f7337fa7755" },
                { "fa", "27c89e06c354fc09b7fb028faac168fd16b57c11263ec7c669a768ebf5aabdae4669933a4aaf3431002970a608ff755337c7201d3e48dfe60c7745413499d0b5" },
                { "ff", "6471b1996439410351e0e27f03008630bf0289165fa0c579e89cb3869ffff522f83ea2dc18223c56750cd4c20eae28625c7d2d8d861f4c92e06e74cfd99bf7f8" },
                { "fi", "0357589c0037b7981043ff694012254dd1fb0ac31db7c98b005778bbe70c4feff622ebb270ccb57cf0f10eb55c4cc74d8c6bedee9bb0cd8cec76a8e91cf6d4cc" },
                { "fr", "986ada267cc24c2bc8dde51f7642ec084b41c14fc3254a3eb340653fe488577718c601592af9e23c4ba69afcc87022efffaa738863882ebb7912b8e483fbd2d8" },
                { "fur", "7efc4909fe2d8646343c6291c295e76ee3a70116741b7d827cca44abbe867a9d62b9b9539048f9d1aacf5706b55649e4c8838934644d3b808fd7366245dee216" },
                { "fy-NL", "92a04adfc59f085d098e604e4abde4ab636d059f5c58c7b93dc7a350e5fc59edc44ff7218deea75b876ed3e98baab24b9fd5760cabfbf3d7ba7306295f0384d8" },
                { "ga-IE", "d86349826437ef730bc2877681a16b20d26274e47f5155fb74fea5621389add8a9cc6a1ec92b11c4fb55d67858ba7577b8d8b47a6ccc7a3291e6cdc78ac7c104" },
                { "gd", "43b6f5bd35a587198a48df81ed2e6e382b408a75607818bf8b0bdc9d3ab76afba432350c35db41ebe37544fe130a053e41fcb13d2cb013846cf9ede6dbbf7136" },
                { "gl", "8fbafbcd6f6a42acbc4b0856beda0c0a1919851070779cc4e177be45bd3d75b4169f61ad8c8eee84b260590d781b93d7f8f73a79ab4e026f087306aaf8c96250" },
                { "gn", "e2346536a898d788917884fa33ea131121e67908fbd2b3c55e1f1fdbb4b579291a50a82d840dd1674c64debbcb7bd590e6632a545813355754da6bca694fa480" },
                { "gu-IN", "fb0173b76de8157f4325933d28ced021319c79ff865a6942b8ac5997a6937574c9f9326280dd5171c6b7ed4b5a51a4c559634250999e1c5caf22b6e2d0b8afa9" },
                { "he", "dbd917fa6a66985daddebf5ddef4b6f4781692d7086ee4d96a64ab49e3c63a866c294d8981c7378dbdd1118a42b33a571e1c5d82a9ab9db986255c6e47b39f4d" },
                { "hi-IN", "7fea907412089e0badc458560b1d1037c7b0867841c6b8dee85cd0ff1718db8770bf7f811c075a390901f2a8d14e45448c4af58647448b9f6553b5bd2e1ba5d2" },
                { "hr", "88a8db306c852757d7623f55903020874dc56aeec48c3c58084306ee4e410bdc10a5718826f1db729c53e3308c3dc977c9dcdd5276b7b53a973388389d858432" },
                { "hsb", "8757cad4914b78082ecf615221e1d516de922d57e0817430a75ec6d184c616365ed74034743fe932368f97d316d262e3bee6ddb1c78aa006324e80acc36f0ae2" },
                { "hu", "c9bd5f9505c4dceaf8e04a21b256c69e43e32292e63cbf0f3b4b0c5e5bf4d0cc408d98b3827187a035e0fc3f0b57bc87fb6896beef620733360476a0fb27e885" },
                { "hy-AM", "562c158ecf7a75d61951ab2148047b25ee51ccd04fc34f94b1349fd1cb1a929dfc9862dbb9d475ed4af0388776186df0fa4a2dbf757042f30338ca23f8834074" },
                { "ia", "f07d838ddebbb763549b166a173e25c4d1d9372adfb8cd0a092a707cacc792a35128107acf76cdee8425db680bcc087d709f86b997c1ad05236614b0a08748af" },
                { "id", "f2c1b67f474c8955ac659b5b5088086cc46efa94087d7833531f8d4608e8529717723cc2ce972f1b1623fcd21fd63318c8ffcf1e1642fc7a0c6045f72d9f555a" },
                { "is", "1ad1fb55a6826835db64e07a3a7c1a8b90a09234571f335107d7a3bf61157c411bc32290f0f7d3e060dabee39e0bf25bbe2a1f4297560490358a88521eba1a61" },
                { "it", "3425a0370f937438178d809e637ad5b65513b4e70c04d6c5119cad3ff0438130552d1f1fac94ca4c10ec420e3f41be01864d703d98a8fe6367bd25a63129bd82" },
                { "ja", "aa82807efd28e6d90facb15daa609cbb318816e36dc8b7f0901c11e948431804f7ccd86be57bf1fb7196b6367a525775ef46640973da60fdcbd9bb45b2a168f4" },
                { "ka", "9866d5837ea006340c282e37e4f458f45f8d4966382732651eccc2ca1ed763dd933528a664996423e4ef7f419d5088879e27f28a924840d3ecff57e7d4259f67" },
                { "kab", "2b0cf15faa2fa22d730085701039ff369305a8cd16bfb2f3c87850bdf533f7dd16fd8529bb8870ae11fd8e392bbdcedb83aa9af2bdddbd30a95ea248f3b685e8" },
                { "kk", "76a09c7dd4801977494bb55f18f91bd57d6024e0e3736ce2b080225d46049996ce187cc958bb7cc4f7bf701de1032431226edcf0bee000ea64d8f6ce4e417e45" },
                { "km", "89067e517884cb9a897ab74442aeea70573e2ceac31bb8841a571dc9258d761ce6436bcb6eb72dc32e38a9fdf194f5405ad4f5141073b3354bab2fab74775f01" },
                { "kn", "bef75441c592d5c4a8a86b84d60d322816f98f8eb5b5258ae7ae51419871b38f1524bee0547d1a840072054e641c76f3d9b2886d2f474886645a3ad29bf1a18e" },
                { "ko", "d278010e30e201a590e2ea29fcf3fa272abc1dbea8ec2f5f8c307ebb55aaa60f6c98ec83110f5261de94657edee31e6f43ddd931452dfce49372396208880e87" },
                { "lij", "2358f1ff491c929172a80082ee49fea6884bb180c87ca11df858fac6675e1538e097e148a355a5b9ec2dc4ec80c6c6bdc922a3ead726869f8f15eb53354eb010" },
                { "lt", "9241ce7dfd126e8f428ac649b6940f9d68c56237ec5f5ab42c0314eca055730049d487e8c3edd2a0ac1d2ce178a008dfa3d1d154de8eb8eb1499e7b08613d6bf" },
                { "lv", "786ff71e88eceaa8ac384097f06e144929f18210cfc1d593332d162ec1a91544c582726df1dad93100fd8c9374c65ccebdecd8e23169ddd4bfdf5e0915abd228" },
                { "mk", "75d2ba16b06bbdb28943e11043ec0dc6f637b2808d9c566af3713e27732bccae58960f1320693e0a0705602a620ebd24ab431e093983a26b2b561c5ed7ee2d43" },
                { "mr", "7230c059dcd5b612618d5af65e02b174eaba5f3ecd290f44a2ed173e5fd504e68546ea2f425211cc9b147d84ab87472ed1fe071030ff6c5ac004c36ce2e95253" },
                { "ms", "5e2f343d2b6b086770842a124117de760275fcec9ec173f94a2a8bba70adb0cbac9847d35d9780ab03d63e25bcf3ec966044c9e7f905702421553315f3272f6b" },
                { "my", "2a9521bcc1c1b680464a42e39d788e06e2e5fdb14bb8ad325a8cb4864f4b312f672c604f344b1e4a6ca3e187a3a48c0771e862e6b2f762ab2b1215f4321f0f23" },
                { "nb-NO", "7646a1a0b38f0a1a13d41fd8a5e95557dff1f19c82c3c5c78c4f5a149ba450ab8961d465374e4084580ed303550d2bfff72fec22b022528259ae3dac04e774c7" },
                { "ne-NP", "fde37ed566ae08d9ae819542ea80119de2687f32ec98599073744b3c1d15efdbe7fc4c8af8c228715d5fdd72d6e58391d8805496cee4da23363c266b762e0bcb" },
                { "nl", "724b4eaa5fd4b3af5d67fc2301646ef001d93421d28803b47a6f9b511fd3442e19fba76004b48c33cb4ab7c352508870bd5ea6868681a4482bd7b88b7bc96b4b" },
                { "nn-NO", "e1d33d11b5eb5e6a631c56b02d769294cedc1a62653b72e71956a8723cc99e188154f113990f8a2b8f10b615901ef24da3c63b0f190d02a10c68da0373bdd312" },
                { "oc", "e148165f80d758c60223f0959b0ce5e3222aa8e71aac7d935bc33870feddd0437d8c2ea8ce63d5b1638927a4e6a0834ba0d03bbd746e075ce371a4b00d42549b" },
                { "pa-IN", "ce54e52472e5f20f4ceaf8fe6b8c3e9a15eddd7e4a5415857b4f34701340cb4232ea77296917a1a5098d88cd892f23bc7f1f90286624cce0ba3b6844ecc2d4e1" },
                { "pl", "ea013dbd90f8b5a41794c2e307a40e9012a6cd552acc64d61edc181844e03b6217898c598c34584eb2b8930b9e40aa1dd07fa8272a9244b54bb379684fbdb183" },
                { "pt-BR", "c474aea2863b0c1693bb74642128403d7724bef1dccad8eb4d069686699cfbf753946b32aceb42b526c302f387e2a36d6ee799bfab7750f86c1a6be2511c7cdf" },
                { "pt-PT", "4d884bd39cfa58d195e4c18a91e64785a74edcd1b3109bae57780b29e12af5d8f5dac383dd92a5e1d1000fef21f10cf261939e2c1eacf0f3a63c025d392db487" },
                { "rm", "c96a5c90eae9e4535dcc554dbd759f3ed5bbb3d960efc6425b2ea25ce21de1134aaef1816e85e98efd75689285ae5ccaacc2ba62125496a1e5e32b54f5648682" },
                { "ro", "dc96da7b6212e31a8b00eda337c3f6c03bdba9770f656da281fffe663a2d6b552017c23ebf230bbff209262ad28d5a20398c7299bb5b111af4149b8589ad60fe" },
                { "ru", "3fa8646c260dddd587aa89bedb894f468769df0f5ba46d33edeb2ef9320bdc3f0aae826436437900e16b5ab2104f641556bad58b0ab2af2d232115965f60acbb" },
                { "sat", "b34a1cfb99c10ee746ec63c511213b5704778c9c4cab37176d758c116dc0b2f69bd46db70e9bbf483fead01fd2afd107dd226eefa0644e7a92d6375829156cb5" },
                { "sc", "b69f72116e3c6eb8a93792d390560ec348dd5967d041a2ab99d1328360f9e1ee60c90dc80716cba603207aae9cbb84fcf9c8eb1fb35e5cc5f0d103f2e78f7867" },
                { "sco", "9b299a193b7e7fc424b67876622aa810d0b39bb3101a0f774b59510909d211af503c94e8e012c1ebb67aed7fa4c0ba917f8c39a2d63b588ccb720d5751cb666b" },
                { "si", "f2e3d75f927328c80960745adc1a2b9f1d7ba67273de25870c2df133b85ca5d386f9b5cd6b125bf9c0de68b1a696ee732ea9abed06339fa51954e8c6264c71cf" },
                { "sk", "7c73b00bd769d66ce9ff7afabe88f8b80294e25141a9a913c7831bc1d066e50e6970a64c71f5d264a3425cf2c49c656c8ce1ce42bbadebea8fe26912970f09e4" },
                { "skr", "1ad4c4e90e334990fc8d4e7b0e6be1e54b117e1b33007f2bfcfa2bb4a017b2ddf0e009be88093b78629f28730652e61c1818722da26675afd3b55190958be6ca" },
                { "sl", "0431fc73ed62a68b36db2015a4480133aa44eb7553da8bab9542c23baa7abac14a31d46b481c05b464214b7165fcde5ef71e4ce45a689f04c46e70e0772012cf" },
                { "son", "0dfe7f422f7945c267705cd0f4f8a9fe9f8e3e7f62f6739da02b18ca0815e515787b24d31315370abdec699a39c9d83f94b867776da9bc83f440fdd1c30c28b7" },
                { "sq", "32fd2dedb47619afb7550a4bd10de8e2345f56d404d2fd7e6a16e2016279c5d2843595d774458bbe99e2505ac58e43cbad67e8e0d7fe191aab8b1b276127d79a" },
                { "sr", "76eb59a506a189d36d1f883765b8beffbe951a391028547d45a3c7a845cee5b0a3a7a0d8f367eee8ca0f82af9c030e8cc79c8811a18f56d5b6ac59413727ce35" },
                { "sv-SE", "872e12ce6bd4a878089e10e25c032f1a179ae305744700f95e6a3efc8810a7f4ee83d4e53acf3bc51c3b4a7c4d434523fa70fcb10b5bac40bc7426a74a9d69b9" },
                { "szl", "13cd09c0332c609384878df8acf5b8d52469050d179015b1400285428637f4e03f96be0dda01a1d1c00f4672fd99533da56a5018355e7a418de0f1b6bda678a0" },
                { "ta", "08c0ef2bc6f5d7509f075bd91dd5c90cbd941ad631d475884ab8576d63652d7c48f0abb6b1bdee6b6e5251cc71fd85098d5c727fe83d6b73eb4276b7cb8a4f8d" },
                { "te", "59095c2b5bd8594a92cfd972feb1c7df3838f724a54ef3c34eb8d15f05e2e3e9ac2ec97062d4b245060d1364ff043e958f304db2bf5ca291cb7223d0876fe6d1" },
                { "tg", "9fb305f96bd404e5db00069e7e3eb46fe6359245ce70ff9bc3d35add0bfaae0d1c804e5099bb832d632a405b61f312d21e768dd653c5078b922e73714fb37078" },
                { "th", "10114e618f8404d4c394a576bfb8e9d065e5099be32e7269a6fbc5f4b797de1b9176699e8a3aedc2a4b3ceeefb68adce5f352bd5ced03b853c6fa08c4ea96456" },
                { "tl", "b1ccc622972a6557debf274906f8a6d1746e6c516e2fb4f283760fb95fff24082c7869106e1f91210863f27d5bdcb8697f410fd86a63c0a4de12e61161cb2525" },
                { "tr", "00749398749086a69135036f5e1dbe7abddc83c68b6e7e3695e0c48f2810d35b412b46fbb6c5367a8d041bdc0bb1a3f444768b837d37aff9bba319583e5610b0" },
                { "trs", "6fed00318728a474f932e2b0a9831d6c3937de0ee9b55b0e97f777fa06b4515f782c81df08cb7509dcb51a35d58eb563020d96a66ca5015e29ab562bc8ea3ffa" },
                { "uk", "07ed4549c1902c61faf3af89e51bc660f6302d2ab109f33b1287166d1fad1d88b6a7495cd9aec500e9c6ebe8ad428ae125c85b12f886a387f4a0ba9a2756f927" },
                { "ur", "a845508d04a19e510a5ad982fed90853608695f2d2079843b8217a7f824f72c4f1d28308bb8f8a82effc59c3aaf45e531d1dbef6c63d53a91a4b8dcb0e887f43" },
                { "uz", "26db709bdcb9009dc686f4b027bdba3e573864f07609026f2fee9be035f29ee93a766a2653f7b74f919a914239c508b96de3c5d0c3414680bb9de1c349bee513" },
                { "vi", "06d3a006809a40c8f56fb5a7a7ee7d73e0d9c68cba82af00607cedc5a6e720e504f7bce79b2c58d6101700de8d37e0747af9ded3a6056340764f9eea3e2f5c83" },
                { "xh", "b27d9723b74d5b02447b9626e53ee17dff1db8ce3348083c4e62cb67410022cc0b4745436ace4d843daf5a8b8970b037cbf62acde549d3614ee6f93f209cf60b" },
                { "zh-CN", "d0d30e95f4efdbcfc5d116f7788092f5bc70e2c7792e1285cd474783a603ac9860ee63d6afc2cfe2408b389ebfc68d1720a0c82cdaad86fe5baa3629ab60a4d7" },
                { "zh-TW", "29de25dbde3db559d3e5c7dc0a2df4e75b0e2cd855deaa9b22554af4d23a9e84c3613ed75c158cfd2c4b7f9f4fee7236eed11bf05b1f174c58c0f1a2c52bd934" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
        /// Determines whether the method searchForNewer() is implemented.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
