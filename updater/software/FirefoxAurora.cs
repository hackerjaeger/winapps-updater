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
        private const string currentVersion = "116.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "dcba5f4e97207bb540489da8aca06515ecaca34d98d19b9db607fd37c7e5ac91c0d478ba0935ef1ef5698740884019d67f449f10c24ca38fb0ab363c564fecf5" },
                { "af", "a0bac8b0d63da0c2054e7cb6d907b28be4564ba1ac198d665828cf80c33237516fba7d375f109b793c2307ad388aea37a83d6a46fb3bb84d9f3b447f31a851f1" },
                { "an", "a163ba55217f98b8fa71cc96b0d2c0ebb28e46f6cb4920314ecfdb973f813c7e5a0701f3884a238894e2326e2f5b9670b672982f3dbc2a8d3b1c0e90e6c0945a" },
                { "ar", "d7558bda3ce14f3a461b1ab7565adf243c76beb439d048a60a702c26d543393e6e26089b1f5fac0ed157cf04cf88a7ebb3c3b7bf6bd481d851ab5ca7c412cc6e" },
                { "ast", "54acb6fc099e0478025e18de747068c4211bd7f71d19b9615ce3a0c13b7f66940f5bd38d14b70ad58b99420b2fd327e81a4e9eb7b78805a92f87687704debcce" },
                { "az", "b0a49b518b9256a8fa00d002dd1664d284684b5c72b994612792d8a542357e34ade5ac2dff945cbcc2fbe0bd05913a5eac2cf0248043cb22502e7670812c68c7" },
                { "be", "6d5985b242010d0c042c88ff06d8a38539c2fa2e612f551406bd43aa3e201875854e2da415e5080a2bcaa7ae871e8a5cff86f4964f4d619d75e1813ce17a45f6" },
                { "bg", "49b9f7cfc95a91009a1e35c69dab2d0716799e63a8f8e8b176dc215cad989de76cee373fd62383a508e46fe1ec65152c7a91a6e8350ca4917dad97394d6c3915" },
                { "bn", "8558fc2280b5768d339205bb80dcce832f711f0541eaeea5269786576c8b0db7b3f72e131d4fa15c25d978575a193484c5db693edd9be49deea18eee29c9e0f8" },
                { "br", "500daa3769ab938da883bd8f15b26e7c73b3b2930972b39e612aeb27bf0c9a8a2a0006e6eb6f5648a4f1f8c325e23f830606541027f2a16136f8dc49d930ff88" },
                { "bs", "b74aa72f2ed72e09abd964098e575444c4d3d33330008a80a1f1f32c0046044c4a7a7a385d1a09a399945c9e6aeca47a590d5e6621bddb3d2ddb342ac07a8c2a" },
                { "ca", "d4d33cd5b17e091adb84ef95330ad5cff9573bad22b933a18350fcb5d084a568672d0f3c715599a54b277163db01287afaf0885f599a8891170035a9d5b93a16" },
                { "cak", "8c789f52812d9416230471d5bf0e953047eaa8de800e784e81e65b8666e1fe978b14506a2f9659643f55696ac4a25596d5e35610cf4385774ad5182053a16232" },
                { "cs", "01dd185810d3a0b721c0d7eaf0807fc70160919fa412d08370fc97c032e87e22c89c5090065bf1f726fe4a1033e6819fd046544600ca8b856794b4c8c885b738" },
                { "cy", "01e17a0fea05c77ca9a4d75000d0e20b0c0311ed11f9315566fb6d21ea0ba8b9797f8d79ada0a6e7e5534a3e1b9d56352aad58cfbc995ab5d7588cbf1ac12099" },
                { "da", "5ab2794e0f448d72d90d14aed6ca966787046fd5b250fd766750487a5087cd6febbdf5f0faa3c24ccf050bbde5c39b9050fbd2e1fb1d08569e2aa0896153ab84" },
                { "de", "f9ce1733a1b74f86d807874d71f1baec706f3014806731341ca27169562d6fe17783cf613b9d174436535aa2fbca31de6e270ec366fe989d3ac8a9892f762e3e" },
                { "dsb", "d61f810977ed89296cfe04a79eb70760fc3fa576da0be22525854a2604903868436cdf0fe4ec1e3b092d1cae11bfebe4846754fd0bf830999d48e08d9b0d295d" },
                { "el", "ca0d06e45ddc83f3bd38753e898810b23bec10384b6339f342624cb40f4bb8404ee785fd221b4bf80b5000ff97fc5ddd6f4f722a68e988f044a724dd42d24252" },
                { "en-CA", "66d888b988b1c86bb7cfe2f0ed7ed0b92920e62310efa2638b2faa1b4b9e4a624e9904409de25931da394f04eb7b0a1f368147b067ebffe2adb5c15198b4c342" },
                { "en-GB", "14a83ab8c09efaddbc7e12868a0c6a65137b2a24a23cc419d17619ec5bff577417bc1812c24beea571fb76ff527b3b55a3c3bcd1bff39c0e7d4c5c82ce3cf147" },
                { "en-US", "b19aae8b786fe9fab2dcef469f760beb9b17e945a94fa168b45e46b722c4f7c1f10a40600bcf4010062bb6cf54256b453f6b8760c5023a9ed17491325e97e1e2" },
                { "eo", "c6013b9d76daf8ffcdd5e0ee65dbcb0a62d2083e7aac36b2df48414219222150401162fc108a9756b39273e1d5f05a2ececeb0b8933ad4609fa9e465725b68c6" },
                { "es-AR", "5ad579011bec4d7eefd75d86e1c050d6a2cc3bf8700d4a3b3a2d7892314a312d04bc7e97e02c16987451b533277ca5226bb41aabbe5813d2f548aa652ca19cab" },
                { "es-CL", "e7b3e2d2ce6fc5bede30e94af928849da1fe52b3918d3d93e4765ca756adf1f66e048463017805ed235a0b791f1b7affc22b6d0616f7a98c40606fdaa4980a43" },
                { "es-ES", "c6b1db9beaad95efac3f3bbf85380219298c15dee5bb745502758e6d60eabb2df47921e831ba56d471841c99d2f0f9da576d47cf03ca4c17d2278057eb2f5995" },
                { "es-MX", "ff50ee2e295bddc0de4dc7d36e53a04a9d97d472ce8b4828d810e319756118c3c2a92222f51129e9c576e57d7dbb00b44f91b4637543c0bb0d15561451e72569" },
                { "et", "f6f0e79634f4030096c34e502b740cecb6c501d7bb310a0e570343f601ae2d8f399b65787fd75b22620e0aebd48f275ac70d87c5bb28ebdadc74bd597a76e122" },
                { "eu", "46e4fd0fa542587afae06ae5985134631e6e831f4f6b2257ccaaafa0752b180b02a442b3d97dc27f1bd606821419addc1f8f0e2e6b2df1636c57600571e8029c" },
                { "fa", "b55be05b25cd3c743c225c5bce4ad799a8b4e0a067bb4eb6fb1595576f68cfbe2c9dafb1c54bb895158b8ce3030372e757d48ab9973c4dd48f82edcdeceb1d8f" },
                { "ff", "64dd29b163e6d6a2f5d0013026493c35c118b17414e11378e3afd957ee87e8584454f17b1ca8329944d05169415a9b091466d4fe6db96fd1dda14be6a72c03dc" },
                { "fi", "44727256c7a45d3878bb83b782a66aaaf735ff55cfaa9d01e86bfaf3b8fd9203e039f69cf564848b13adfc1db528475c3f30627a5293f12448a358b6204f6b4c" },
                { "fr", "c769f399a9363a683a367ac468f6b9a742a758d6f0cd69c9e5d0c4f6427afe99cd42d93c2420594f6e347e1f6a2baeeacec79e2cee946bffb166d5bb8bf96cf1" },
                { "fur", "9cb8644e448d219f01a0cfb90b0b852cd7a443d334e496ea968120d2a003bc2e3fe1d871988ab8036220adb049ebac3324527fc1efba72e806de05ec951b9d37" },
                { "fy-NL", "d6a8ba8eae5812eeca2359a34c8f9507bd91d8e9539a6e5ceeecb7480882d75c1f44b2d10d9838e738e724803af539e17b5d432b1ef0fc485ee6561d43e1d717" },
                { "ga-IE", "eb5f29b3aea2d9c83f8b31d00bd67cffd060b70ccb9e04c45e0dd9a157610058cdc9806bdb79abcb6d3dc2f40487c5872858ef7567f8a13253d26ea74325fa1b" },
                { "gd", "f840e795401c215e2a4f5f5343761019331bec254e5afdec9feba4687935b8c277f5fe60afc2241000cec127ab6e9255ca9cec406f019f0121ecf17e3078f1be" },
                { "gl", "c4cfa66e67c54327252cb7bcce59e112d09e4c9301ee0879d5039156730d8ad80d18a3f727150a47c55d5af96f15484d307ee43d6104413375ed7b58bda4dcb2" },
                { "gn", "bc3725d36c8a919799704c75ab38043a27870f2e3d4c0e56d94e8246b28560109744cba6df9cc11a94313819dbe1e6a0c97c517218a8b3eda2d26920977be43d" },
                { "gu-IN", "1f41d3e2df58e0ffa10025b9ae20faad827666c29573f89c9b4c5951400cec412e3dc0241c52cb74f18eb6c614be308995678f921a5f67852e6ba9808e001c68" },
                { "he", "ee6541cf811120ad88fdf0b36b4fcdbeef567cab61ef4306b43b188971cab0cd1bba348228e1d65fa7ddf88b48a69496ffd07bc19cc5c853cc48105ab0923d35" },
                { "hi-IN", "b91d009f97b2226bc8aa4fbc3c88db879163cfd35092e0e5dbf6ddfa9a9816e4f018548c0e02409cd2b5ac35d37ee74db6fe5fe69e39735953a18ac149d8acd1" },
                { "hr", "c65f598168c922facc8eea9c10335c94e229f6f9aa4ada1b54b3b7c5d57f423fc1e0cf92cabbd27957a850e228a1a60a1dc3704cfc6487f4bb97a66fd91f4ec5" },
                { "hsb", "5523afcf8978377c7eae2295ffc6396a7410f557506cbe2aabd50dcad3ebf1ea738609eca8865ef3b2fc72b6684344fdc93dc5dda0b9ffc81a2f8456f242dd4b" },
                { "hu", "50b539995c901b9a52d55fbb93aa3c3d4ac0bfc134d0b5314dddd3146e0dee53bcd372e89c30ada857d9d6f838a544f370f7c818310467453a909d0ecf3af1cd" },
                { "hy-AM", "f157b83da2d49475d41bd892168e6e74694525520427acdc9d46eabb9b59c91476f17b79beaa8851c9ed78be626224c7f79f0c815d2c81064683dfa3d7df14bd" },
                { "ia", "85f5a01ea26e4d3f3add13343d6b499072f3f6515bb4b112f80414efbf62cc8cff6988f2430d0f4a7230e7f22573a032f59a6fd6f0da65317cf29f535acff6b0" },
                { "id", "851cdfb0eb5de1fb44dfe75ec9f0aea019dc2f53058dc808fcdde6f9e7e6ae9d9e9c916c8a416743469c592c7c39f5139993e9bb76c5b4f44e37b86dd50f588f" },
                { "is", "10e43b922883fab96d2afd01223e6dee899e759930db69bc5a4ee646ed636d377625031f433f8ac19719a4754c3e05b7aaf5f1f8fddd5648e6e73907a8cfaba6" },
                { "it", "64673db5fd8d44b73aa8295f92e2770bf81f5aeab3e0a250db240b014bba63127316dc68413d1036ce288fd7b5f6295f02740f16ccdcfb1bb94984d45f79ad5a" },
                { "ja", "fcf6c416132530c60c606bbfc6f56ca6a515e45bc3d9d928653c1b50cac67790efc95380c4ef9abee3a3527a78e6b280b3573dbd6523aa5098f424a536107859" },
                { "ka", "38694a8903a3d70fc5c70bc97651e78526229bb981581d3461b7996b995cd08378b4d9e6e8f86c5fedef1e3b1b775f437b83c9e7315596b716377cba64cdd72b" },
                { "kab", "b5fd145537c1ff16cbfe389bf1b5ee062874071955fb92339a7f59997cd5fc35043587d8a0361f2e6ec012710b573d5256df2e076d5bc47a3a35210bbb781745" },
                { "kk", "bd34150071ff5c7a37a2026104b954bb54db0eaf264fecfb42c44c05826d9acc6553d38999af807a91e44e759b7078a9df77c0cfb41a121bd3ecda4bb801ece8" },
                { "km", "c89a676664763157870a826c8731b4d3723f3e97275d40097e6d6f17b97862c2ba9add58432b1434b3141cc7590e620c37d47ac6b370f119da7a0e647e03bf98" },
                { "kn", "8962b0017adc5cbaac269b64501640465d2e502c4dc04ab06a356c2a6cd7099b36ff1b736e90bd1488a9fdc08e79b37cadb82cc99d5c583fd21722cfc4d1ab2e" },
                { "ko", "b1e7a859a6fa3229f8af96a599e2a638203d496bc947510f64a0dd9aad5071e8c29cc3af9a4b5ec3ab89fe7e1daa59c5a2ed24dd1e5e781867b377804a9f5434" },
                { "lij", "64bd69a6fdf5ade3c5e11fac0093fd6b6fb19570820d965a0abeeb372a86a4288c98bec973676aeaa177d889bc8bc92ce424c2eb4230cffe118d2e3a6c04808a" },
                { "lt", "c9fb025e1f27f655c680c632f89d718b71f361eead4f7f829002e358d7c63d4fc9cf7d9e9e7efa1e41bd0a14240a2ff57d55f69d0f62d0376cdac9febffb7f06" },
                { "lv", "9c15fdb66675839b9b561e16f42adf8db11e88bd596d0930ed32026a7a167353c143bbbc8e9df8884bf935e908a7c6cc511ec8591faf6d093e836f0463581db3" },
                { "mk", "171f22a997b7937ae11cbbea8b92d88f34cb4320ea161bdc5415a13cae1596747d8045a584b0e13d33ca8bd644f9469c8f7e780deba7d47709d3457d3b622c99" },
                { "mr", "ed7375d3557b71fda0449e162b238d70ef352cd905c5953126ab2bf8f932e1970f93837d3de690a5d1669e1724db900c060061838e2d8222c72b382b5fb03667" },
                { "ms", "cf295ff3ee5bbfa879920dbbcfc61ef384fabb1b933b17688365813e9b9b0868a70f139cc23ad2d1ab3b8bcf93ed5d54db066cfacb5fdfbb83d6c828025e1ea1" },
                { "my", "ddb3f1a4e872fae0e19473263d426bd4cedf6ee09710744fdc1c59a088f0ca34a20cbfb2244faecadede7d82f9402448d49a8f846506da854acb4b0400a6be04" },
                { "nb-NO", "9ab90118c21a68ffbe0d4f56f2ff75c83b761bb9b3db3e514d472d485a426f9fda89380a501d32c7744abb5fe4beadb5c91917fe71fa0556a451f8e74e579daf" },
                { "ne-NP", "b490553ed663c29a12e70f6447370099e7b616a14d5ce899a478735e4de517fd4226ad25877b3183b56acb4c4dce8252431c1ff5b6f38435b413f224958bbe59" },
                { "nl", "25aabc7081e8c90ee6e072f6eed05f342caead27db7deb8add79537c19530893b56ec2e468a27674c21d4f02da482d9c6efef4c62881cf7d9287e569eb5110b9" },
                { "nn-NO", "b89da1b16fb7498943d202c36caf4d6309aed813fca281d9b4f122e93c9ae3a9def052cb6a110eb807f3e4a482ffc2c8931f1902dd496701b51880a27453b29c" },
                { "oc", "3a81910374025da1cdfd962517517255727bc5f3512b990a038ee387add496b927e4a00d57a37f7c5f09bbfa5d73b748cdaa28c539b2c445000ba4e627ed57aa" },
                { "pa-IN", "f6f239fd003bc30409af671ddb16c127e47e5f88cac0698aa794d97869e0b8d614f3ffded3428e908dd8e6eba64ae8b86bd273ddc567ae5ff1647dfa5f005e77" },
                { "pl", "2077a091ab2e00bbdcc38cb039973352d390b35de8ec621c080e58be34481de0b143e7bc45c69868336aecb57fc68f2c221f41506dbdc032d448d7ea970db8fd" },
                { "pt-BR", "20861e2625d77a54ea7851518d079c16956cc45c443ffd5b851a282be1e06ed0da40fef002ba791ffe2438e8a4412d1ced964342474780b85b0e908e44e98706" },
                { "pt-PT", "29a8ea1093bb4d0e5d35ae3b7a4aa6621ad0e9b32e939a1e709482f352281f7d4bcc23032a77bac6061d854e1e6a4e86d7079c3ea91be57fc213f7883df2f5fb" },
                { "rm", "e186e89d8f8c75a37f562d40fcfda93e146dc1bc434ed5946082ef58abe8b790477b87869e3fb733ae242f652b7fb6053130474a0156c9359dc2c46666271b55" },
                { "ro", "5cdaf4ed8917a7741856b28c2924384b57910ae80143719ea111906122b76e4bcb8bc74fb0f7ec77f2303d3af7659b8e7f8ebd3b7d012190f0155459cb12653b" },
                { "ru", "e85f69116665de34311058ded335c854f3ed14b69891120a0a1eefac83298df87f8eda4d115c705b748f0423d390a4fb53d5de8ae61d8ac8066b3d2a6f96daa9" },
                { "sc", "88bc8a7785588c98086efe40500ec57f4f084cd4d13de27afa74025fc84717fff43e7b8401be1cd0cad31545dbae9a3da3435b9478d187f6e07174d4cf8e08ce" },
                { "sco", "9ae03ffc3835a2eaba78281e188e399fc6a5d319118ea793968e182d25fb1391e09865056fdb81d0ea1a3031a55860b3a3caf01f54f3a0188671e9c3b8fb2b5f" },
                { "si", "f3014ad38da3630233bf8637e97444234f93776b4187c9816502916c43e12845d2744911ec7c82a7e53a30f1c30f4c71ff2a4db7964c72e4e373d9e23d260a94" },
                { "sk", "cf8069292ecae5df3102e3e5f5f528dc13fd1a37e8848e8160551f48d574a10c64f92b0da3a44f242f3589f0471d503e082eec1e4da3f7f4427b33eebf1bfaaf" },
                { "sl", "8023fa087c13d1b712f7bd4a9ffc0af9d00b7b13594645a38d4c61fafd1df60cb23f9cbcb56f8ce67eed948b6fbbc6e9c38ce73ca9fdecea6aac4d8229cc5c94" },
                { "son", "006752737948e885b2a0d562eab4207981c44a86f4d359a226673f020156828fc5d389f06dd3dedec834cf16b622a8f32b1b32421111be584bd4faf05923a0b6" },
                { "sq", "271d9c4200d18842ac9922a337b0a807132484ed55887d277ca4227d592d4a74e6c0de73c7308dfd9cdd764c18c9b14faed65f0cd507c66926340ad652f73b45" },
                { "sr", "bebb34ce235d2509c0568a30a479ed765416796782ce430c1b18b010b7449c24186bf3213eb6ca3000729dcf560ec330b1e6e4f6c03d1792ef6e3e510b3bc048" },
                { "sv-SE", "7e2876f891cfc92da70fb64a4b91c7161cb996cf00e2bde45c8bf25d795951eea493ce7bf38441347972d2d6f56e08e44d2e3cd22def561c6ff6639420b67c63" },
                { "szl", "5b016deda744e82e6979b632fe8720839162c21f042b7176e3d7016a6136ec6f2ae6888bff53b521596dc76de7285919caa55a17c64a780743fdfeb3d245f57d" },
                { "ta", "02f47ab99c63d3133dea203725020021fedb48731c26a5b99f5b0b025bb785e265f6c4e2628f5120fd1bd9177edfdf1350d5b9ebaf384b871db58fcfd91dc8a2" },
                { "te", "2565831d59b966fc7b6517aa39d55bd2ae439fb6924c7cc35a8581dd9c8c7b08b42e4c48c64d84c46a3eb8488cbc7e5840bf21cc2f326bc9120af3cd55312696" },
                { "tg", "02001abe6f4d9c7a0989f15e7430fb582bbcd5c149a60d6fa95c1189c34639a6cacebe4e1e3191ad086c48eba5d20a5d0868815d6ec39bc2fc43ec091bebccbe" },
                { "th", "b44a53909e71051c4ad3c8e21d192edb229aa739f414209d0d4f57400eed15a99c91bb31d953a92e936f6ef4b3c20ac2c0b7c90f820e8c9f7b0e6e258a32fb62" },
                { "tl", "88fac69ee1752fad2c46e047bdc7ef6a8d9a528f086a0dc8af4b9026f294a8ec6017f3d9e6244937ee67890e320bf93f20f482c517443ebcd6c9d65108b83305" },
                { "tr", "dd5487c7e882c2410e9163b7b5213d34f6cb9a5f266a7fbf806820894043e74a70c92854137ecdcb6f2b90fb1ebf76cb0500f05f5aefe1def31a72444afc9455" },
                { "trs", "c11eb48340395a7c289a9e684d76d470b065a545eb4882d4f7f160a1420074f3239d43aee1388eb4e2df81292f6d65a1ae35e15bdbef2c8acb96c02327006cc9" },
                { "uk", "c590af9268bb8da3bb7ae3642522d77b9eec2f6f72402701b91ccbd1d2351aaef960634171144bc7ee0475ccdc704331febfb1bdf44227c496e730b8c6e1f21d" },
                { "ur", "9fbd69171c43aa1412b5ecf4c5980d9f150e5392c7ba03ba9dc3623e00166465e0bed30aa11f1a0c911797c4ed41a79747d5563411ebdb8cc11e848b4d12449c" },
                { "uz", "3bc8228eb0dff46d4f8a86b2aa6bd6c44e12fb768f1bcd3a671fdc030693c2bc98493db0009ec91778793b7100393f32d0ade3cd49b7b792e00b1c22677f15d7" },
                { "vi", "0ff136156c84d0cfadf7354ca7cca4859cbeb94a269137816004ce213aa5dcf0479f66704998bf849c12b600daa9f793a7cd2e8eed1c9f56b85bd7969a6c47fc" },
                { "xh", "54682036306c28cdb151d36a7098d3e91021c7336d42eb70f607c73cae90f7f7872b26f0e200984d9dbc99f7009cea1bf26f8961a893e2eeb91873377c0177d2" },
                { "zh-CN", "47090a9fda9c52e64f8588ab7e6bbe9020f272b38fa0af305bb3d3e1f15a66019eb6027331b875aa047be14ce1e4f9ccc6c3877f62259696983ef43b564af490" },
                { "zh-TW", "9fac993f75598a9fa6a057ffe84aeb2dd9973bc51c477f29335d658b69b27ca9eb23f92c5a7fb30b65b62126d14f8908596ab58bdf17070844631c95bce3ca85" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "85f7c6166be4f36bbd25909cb655d9c4c73a86572c468a02f6a131fd414f2e40c080696e06f8caeb3c54e8df8b8eba22a3a3dfa0cfaa2528fd33c051ca76287a" },
                { "af", "50631d6032d455d8f9108d75949780a0895bd2c989421f10e5e62ed4a68463fc2af4aaa899a2690b9b2ff8c615d95282b57d7723d9d16ede4fd50d8e39e7d6f1" },
                { "an", "a36e51bc1e7dff8d7d803e09c97de37be05202b861bdbf43f694d9fa997d724cb0b12f1f18038bf7751198251bb3fa755bc07cf2b1f0b56bb4514d993d16874e" },
                { "ar", "e020fb29f11581461e97dcb2804d3cd9928b86e1b4ad7d7728063e9ff8f23a8c32db207b93139aa2ab6f64481f4a289ad6b2fbbd7bba2c3d0ff34a6d3b019f33" },
                { "ast", "edf17b58bb650bbff24f0f35f461b215e85c71162b6d430786517271a7df1edf31e096ac308c366c2e609b20172b92a7e0cc817907cfab85732ba31e86c22911" },
                { "az", "abc304483270cf91d29a1031c2b23d8a5c11e364875a78c09a17870e59b3efa4f54dfe995eb86a52b730ba727757bc67d37620b9a72c50b4b9216673638c8258" },
                { "be", "52854f257b7ab3ba89635d928ef972374d27a7dba983ec48fce736199e61680d5094bc77224a06e6d7d71d27e6e811c4b15b086ef39b1934ed8e205ddf940995" },
                { "bg", "7e14da2c67b50fd2244dc6180c4a9af93d7184b91a715c4287a01daef65e52d876d04d7d8f049af914701c8cd810d4bbf9fa3bd4d7fdda32f379fc83f8bb93f6" },
                { "bn", "20c95395133b2a25dbd644a850052d93f687bcf315e2a073cc145f4be10d60a9ea4de13e45294a1f987f67a950ddcab64322579f9e416cb3c7c9bbcf643ababa" },
                { "br", "f377e41099fc3eb1ee35d1166a5b850bc38125e6b73c24d35a86b039e6c106ef42b03ccc0e5d2cb7d2610043b22c933907d8c670de6e4873195200f51d0bdc61" },
                { "bs", "ee17bb1bbadb571922f7630a20ece2467acd52dc9952532aa03d80ff1948eed2bfea63152dbfab810134ea1b86706c1c0f71b752bf0dec58b1046ed3185ad225" },
                { "ca", "68721b5a78bf5a28856abc4137ab64c6840c58ecc2acf44cc5b1b94967147801b6949c3a58c773205647542a68110c9800ade1e9d9f4a1ab89d105c5a4525a49" },
                { "cak", "d62be20780853ff9588c984d6cc2cb21381ecaf0ba0166265c403a914e2a728c8f5d8da785c7d2f557c0c33d4e984e0f19839a85f6aa6933ad37c98c582cfbcb" },
                { "cs", "8a173924463368f9cb5853cc8abe7ab29c1890c1b1aec65f7391f56e1da9322bad342e903c63101a0d657f97cf7813aba3d9f1177a29c79475e48622dfde8022" },
                { "cy", "753cb0fde930b6e2e4a1b9bea87eade38166d3786e1e366233a83f95c744c9205ce30644ba870ee7c6b2dbaab7a3a36e57a2d38b7417eeb48877fd298f69ecd1" },
                { "da", "d4c3369397f13958cfecb9408d5ec9644bee6461dae41abff8c78f0ac06d87a70ec3c57904aec4a25548e03d6534f8188521ee88cb521fe46cd41a017d45c7b6" },
                { "de", "5fb403bd572cc68397f65ca6aea285eae7e4227d0dfccadc7e53c0b3ee4ab0b43d4a7f6044baba996d2b90e0effd17040d22c903a7c1492a5561ff4e19b639f2" },
                { "dsb", "f22fba45232ef22e316f88e4683c9d3f32bee3eec3418fd2d46c39dacb64e542c525bb3d886d753f1f774aff971293c27580fef64a2166f38075bcd138165123" },
                { "el", "62e54931e606d83df153bfacbdfb1a03a3ee66710441b1ec6bf4feff2c4e29e8b10a558564be44c9f0e01e12bc77a839f71026ee5c9c0aa14215e925ad9ca546" },
                { "en-CA", "12ee269fd08514138fef62dbd65424c107e300043b79204be06d9ade23ade0a58fcd48cedf3459fcb509430ec2f65cdadd1f646295e9e65076f2620a11e47ce8" },
                { "en-GB", "555c7c6d9a96bf8319f571f899cbac5814d2dcd6b0cfabba5ea6c18582b7304234bcbc4e3c42c66da56484c92360e55d15c6dd33ad9c1c3e92d74bf2ec19980a" },
                { "en-US", "78206436269ced76dbc99c1370488d240e5c4e9fa5cda9e3fc6c2a540c2acbc2583f989724c175f32d7f0818cc706052a3118edf67ea8af49456a42fead6189d" },
                { "eo", "0f473082b270e107fbdc489c05b1507a1716d90c023cdbf3cce34d36050e8c1cd287aa487c232fd292a44ecfda25b55eda8a996b45c59aba167f547b8a9531e4" },
                { "es-AR", "f53ea1849814770a506e3ded0b79c8a4ed3c8c272de2656ad49ffc0f75339262281f5feea490a85e37fda020c0ee9170af8cccfc59571303b94326b73d5e5fb6" },
                { "es-CL", "6147c9c1c3e5a8d70567d9e81ea294cea57f30648b1ae50ab7e6e2ead9a963de0f7fa6b5f65c570431da923b5aa782a20cfb186f8baca8a2b89d773446a993cb" },
                { "es-ES", "9e50e9f6548c90eb7541cba9f4383a99672e615441e6a15ce1e807983f40e9fdf8965a1e4c5669a70780fefab4ff7ed4d1a19dbb3d78b2ad4982a8655d51090b" },
                { "es-MX", "50960aec35424ab30acee7cf3d326c586a4a3635813c6fa59efa9c0de037ec5bf98736e3dd207efaa093556a5466cabfa1c3dd455cf433af0fe9ddab5727f736" },
                { "et", "21f759c3ca78d0146c5ffe055a421f915976b49793dbeee84149e12f0c0d600ab05db3a49fec8b07f7e83505366c870343628c999379bdfb6e063e0410679949" },
                { "eu", "3b4c20257363dc3d16af6ca2745623c6622795661cf2b9f9787d2844fd83d73446379397e7ad720b4bc29aec9fa9a84b54c2beba78094db0989bc99bc7c3ace6" },
                { "fa", "2d43745ed7cf3b45beb92b0bbb10836283ec1066f663a272f534d5afdf537547441d42399231f598faef436c53178d401767824027dc06d771bbc706e3644051" },
                { "ff", "5dabec31b9ad5631978f4a46c74b69e11dac0a593b1b7d963afcf8e0eafdb1606103a58f60d9a1b9765ae99986c6329364bd3a3be8d673613664d97b6498e18c" },
                { "fi", "2ad22a16dea69e62271e10371606ae6d384caeea245a5d106b89547a11ac20c9c72788d108f2be6de8c039bb4f4db5c35ce5574222e25025b2f7d63c59a677b4" },
                { "fr", "c945c46ac862c8fd007034cc6b653e45a474b5e4d76d4bbda3f09708883081746dc9ee99878fc976753486275006bf271af5c5af218070dd47bbc4bfeaaa66c8" },
                { "fur", "8ad20b95ce5da363e0412e48c3591fdef59db4fd87e9c13c9470fe42826f48445a8851fddebe4181bd3e0fe9561e82c268d3271f3b908cd188cf6f1e9ebbbef5" },
                { "fy-NL", "83ad56210c5915e82edb68413e5cd1faac2328096e9a918750afc5fd06187a7ac05bf7411fee77035ef47334baab95d1eec8f04461b80df3e63da70fa580e803" },
                { "ga-IE", "b42769a36d6a9f131e0d3155ddcfd071cf908f3360539ffefe4f34dcd1eb7e997af25b16ce4f0ebdab8984b44bf9e37b1ff5206a8dbf37d21dee754122be1427" },
                { "gd", "6fc0b9aa5201a4ed581d7d9c76ed2c30b0f895695404f198289770507c88e344daceeb044f29f86620997b7ff2bccfe2d46233d0a20e106cffcde7ef3886f7c6" },
                { "gl", "d5527d03144d21fa5ffd7e1deda6f3f58b5b4335f35e9a5baaf82622b5cbecefb0adbad289ef2f2afbc3c0bd0c6612bc36c3794eab54cc4dc34d52f8381fbb55" },
                { "gn", "e53927d4b33c7dd5086e12fa6737bc8fac73e7c4e77085f0072f68d66497cf51778f28ec158c5c674a0974a5dc6130f487e93c7533a1b5b9aca5746d2a7e6a2a" },
                { "gu-IN", "e66523d0c79a6d4f796c1d94088fc1162da4e08a1544fe0874f97d358316d01fa52e221c38e12ab2d78d36ff1f6d1513dea6d8a83b9b602176ef1eb78fa66f67" },
                { "he", "251e54df525ce4c20330e70c2ec903c5192b5bf0545c8e39a87f2604f5a15caf8b77d6d28bc2bf38a3435bc0d5945f4eb9bb2b8184f6f1e05463c5a1ce4811ee" },
                { "hi-IN", "7d2e3fd00ab5a1608b885a123fe2a4a78133c70d2242b1f4fc219cc9a89f209052f4c671ac3b87351edd751770fabfb5831c06a94071fec3b971a8fdcab26250" },
                { "hr", "1353ab2c5770c819e8f8c594bbd6454224cc4bb5bfb1aa81344e8ae8f4308079a48aa15f97374b69c26a9aaf03e79c4b86aec8047cd98acf08346c8277e67a38" },
                { "hsb", "d8b6562b264efe2c8f687467b34564059baff7bc42f20a533670b44703caaba9966c6c5fb1542b3499cc523e452e90898d13092bd06bba62da72e3fce17e595d" },
                { "hu", "bcabb2faec42972c51e14c9415a738ebc2001c2f6539ef34125e468da1750de2c0d58205c39106d081965a43d69ec634ca4fdfdf6e8c19eab12ba0fba76c08e4" },
                { "hy-AM", "3e391071658f58843a59af28a0b5b4fc4f2e82019bbb9c2ee07958fdf1ddb67d69aef75c2b83580a679309e82b79be5e2b8a3b1df5b4ea9f508e4af98e36d1c4" },
                { "ia", "1ac9a0e175906d4d09179e8ae872483903b02480e56a2ae346d7a385b2ae723fe9f6ff0355d24679e9c779431b608cc78a3a7b598545c158c6afa847e0c8d687" },
                { "id", "0490006d0430627a598def864074af3c5e16b334fe043b67493c70b392415507f0740d9f25c4c7269fdff04942e774d3115d22e3457c6f5074ceda0e00e7eb61" },
                { "is", "5a4092412d116282b307204ff6df1ca16672038a0e209486a295b53b86a6f933f0fea92e8b3e38e51a542bdaa3d66ca1e41c8cd0c4bdfb4c0e3bdc5ca1c5aa7a" },
                { "it", "6963dc6a6eca11747f2ba63f2b8548baf7ef135890e102c367fd49115011bd61e8b56ea201f4df49f2bb3d86821a9f45b56c931657276f2873224353723f485f" },
                { "ja", "34230cdde8418388a5f4ed4d828a77024ec4b1c7d37d0034035e4d9aa578c5c42746f89b7e68c6acec8e8703a21c6c79f71379222222cacef4fab63b909d62ca" },
                { "ka", "b0beb8add1f1bf83ec0b711f6fd4e1574ffa80a2000074f925aa1e42ebd2fa7530fc3f16826b1783b7e37ef14271cc3a7b3fe0b018c3b2183f77a1908d2275ce" },
                { "kab", "9f2e69c9633af71ac0d7f33bee066528cabf00b8099376253f80d13c6ad8727a79a22f8b46a8256bdb3b40c877a368322aab8c820214759e5aca6083107d2598" },
                { "kk", "1604cda9fae6aeaa07f91cd9b19f0adffa099766a6f2863c5cd307283a0587283f0dad2fe633045fc41d6e05d7753ebab4e2d68ce67e936ec8f4d5b0cdd3aeed" },
                { "km", "ada0ab3e0245fc8c3342529eb315785a6eaf862e098791f5baeb5984c8117e003cfe37af59540b58a238a2bd0c7d4775d7fee39de8c7d42689064c8f265ca749" },
                { "kn", "9d2b4f83057647a5e280ed7296d9c2b00776d0b34dbbe7cd00f5d680c05f5aad0b8b7891577b7dcd8892df93938c69d06fba94338b013c4ee20f169cf849c2d3" },
                { "ko", "4c3d77aba3f762c82cdc61d392294a9c8c8af52c9f429cce244159443415d2a54f1e0412389124e27fc0ad2c82674145a001e39c05019b5b201abf290399d29d" },
                { "lij", "d57136f207081165000a9e6893824c395a0f7b3263b8783008e03dfdd066dd074f8c8cae38411a6722c549544ac37b24d200e2a734de23c907596b58ea8b83f8" },
                { "lt", "3a1af893e5b5eca7d54ec739c4dc36f8dc972b89acd2ec2d97b07aacc0fe0f5c54fb9bcf95da5aa153a42b94a52a28ac68a1e6aadbef8ef043c2257da36b582a" },
                { "lv", "dae1200253c8e0ddfbd9427bf36afaa97ff273363636167ae7472f492074c9e6f094ac865589692eae5f5f05d70401499cba86353ba46028ae63f08db044f49d" },
                { "mk", "a46d71310cb66135e9d97d707254103e37eb6199e4fa90146af67eefb7811eb8e5b3ab4420e5f24e29b051c47b92ffcf0bc29eee36cc84a7b259fe394ed6d619" },
                { "mr", "97c49c73358e221517a0f336b6df9aa3e5bff6aa138929400a12b74cb01a38b64a3bbca4f5ce5319115c416581533abe6bd5533aaed0eae15ddf5b5976012d5e" },
                { "ms", "673a525fd44718c22d25def07cb90c7760854fc8a90671cd913fd8ebfa02cb37456e106bacb2bf988fffe2b6112e1a28b3c3ea285a5849100c2c4ee6d99e12d7" },
                { "my", "597b907483110dde3d93b39047439f7e0b5933db8d6f331519f02b192ffc874234921f21f870d3307685ad95d77c6d95e572013a564dc20d035cbff280eeac0d" },
                { "nb-NO", "2a222343e7ad7b8d5ad595df2adc7c2963655f808f871500917e5b5af05af28b2ec15bc4f389a663f7e4fc9db894918931a6f500b82c1c272bf51fee7f7e86dc" },
                { "ne-NP", "4082275c3ff504fa3eed3465ffdfb67aa2bd93276ab32d339856a0024c85bf4a9888f6b1539d02076a2a6f6a32168ec2e12591554e39c334bef5c77c57aa1be9" },
                { "nl", "3ce2ced376aa460f27eaf10ba58eabaaec042523294efac5ed600369769065059327738e9ecccd0544e750b4c4e5ad62945a1fdc101145d0355d4ca109ac8ae6" },
                { "nn-NO", "80ac225576e4a7860017431f183015231bf89e73dc04f1c6f0a87125f63ee25f662b1b95dcf0b207c8d0fa8c769fadf7f69e448e5e3e5fcc1849a70acfbdfc15" },
                { "oc", "0e7f59dd5d2098e6f5ff73c1652e2e8e769b707e7263c69acdba5b991d3af7ed5c88351a165d555980d55c1c182b61b1408972d512b350b753abbabe38441ffd" },
                { "pa-IN", "b9804e2a231e204ec992c8769acff28d8033ab877cdf4da08b69948ad9a2e1d10a6da61d6e442adf96fc97c807286388d19b75413ce7371fba27cbddc55b48e9" },
                { "pl", "afe3e1914ab193adc0a7a0b0fc761e0c14226836f9e015b883e896d7f26ac7024465269ec2cb242759308d6fc0264291704f111a5f8b4b610ef26e28ed205445" },
                { "pt-BR", "6ee588d57687a8c40bb055e0cc1a42e6aeff86585d485f77a5a03d220ba79cba98ce54582e30e05b38d0e371102be5b79eabf8908a1e239345631428d24fa378" },
                { "pt-PT", "9ec1662f63a11cfb0866f15d254b53c984d4edbc7223d58e7f108bbf0704791ef34e7b87b3ebdd7520e1e9fa5541e6e0c102ecaa83124c326e9d03c5ecd5541a" },
                { "rm", "4a59cbdb852618ef7d4db84f0792b2b27b7a6b7f398c932a047d0e26546855f6b2597ea1275dac1777e959594018a167e7086f78c2524928f9a6c841dbfb7f03" },
                { "ro", "2cfcde2b398ceb214680629f230421f31ec6f77572f44470c97121fb66d3905d0289e4410c1a97ef74e4dce82a0233de4977b0a7e393c5328b8b30c37144b064" },
                { "ru", "4f0ce592e550a160ff28cee51c42b547544b741f99af5fb3466cdeda8c6be16a4fa5b0734dfdb8f5a3f8e1eff94b2946b655ace667d11e00d7877c7085a56ead" },
                { "sc", "16676ce5193d9534d7f09ea88755ee8fec1d104d65dccbfff04e27dcebdbd3b23555241acaeaea2655e1ee421db314b1c21dafd49c29593a78c4912c6bf32a5a" },
                { "sco", "d73d763120be81db64d001c9b61e432ea958ee0f61671b4edb102af0a89f9fcd754bcd3299e5a2d8c36675fdb7b922dc60d8f8ff5db96a50b1be7c0b21a3415b" },
                { "si", "b76cb378137b5ce4c1ef8b1d530ed008b708655d64b274d0dda1b2e6007c17a12da20f23a2735404720c31e3aca237cb52aa6da05b949d40074b00e95938d37d" },
                { "sk", "c49b2c7087d5421c4cb59cef82389f44c036c72bb098bd7381c25074b39b0d2913011e179dc054b7eff85f4b50822f7876df5f0cdea838ace2c1c1b59b97bb2e" },
                { "sl", "e01f4ea2eb1f29f985e2206b5ee101ff2914a47d32caad23a24b47ef56d12338d68bcfe03abb829b5add0139f2d183b6a4a8bec09177336228e9ef0b66509714" },
                { "son", "3b34753af1e1f55007812acbe0a07452c2bbaa6f827b958cb8023a1b1637c9550a6328ae0b8074e4f81ef9638e98a492d71ae909817cc9cedf9d675c462d2d41" },
                { "sq", "d894e149812dcb8fbab9e70594484b7846feb68b7272bb0bd150248fb0729886671299f9dd9f51b06e5ce167004c8c735a61c23b599958c48b2186c76f3acef3" },
                { "sr", "7cfd135ed0fbcbdbd9e455bbe8ee1cc88d00292bb6c307cadf6b7d2ff5a473ef65b05cfea2a5b672ff9f0f7e5bdb3a4901e66cd2b1b3f2cb9596027a9c2b292d" },
                { "sv-SE", "43421c3edf32b9981c6eab8546b94fe64a248f9502b5f6b797ea4129157db6b716fe99cb2e8a0b3fd5270c9561c4b0bcf8fff81a4098ff1c0c07a257007f2814" },
                { "szl", "f615ad9bda6a8061117c726936dd48c3b21154370e7ab9a59b69c41251ab187a2549d28a33b0d6e718d7d811a796bbccfe92504aaeaa3746cb23f2b829de6c99" },
                { "ta", "4cef8541c04c04b51a184fa00930ce7db4bf3bf1ed288dd8408c7f10d413ea1d2ba2682aad448a48c433ddc16aedd963f2615ea44a5ec7852440b5fb65e43b85" },
                { "te", "2e9f6d9321ef09673fc4246bc0821e4c817468bee50093146b11c26ebddf6a56b8c6994531d4f68447dd687cfe035eed7745fba923a610fc8da09de221802375" },
                { "tg", "1e796e31bff4f85358fdbf931b54ad9c596f27fff20818b9234234949354424d68c2758e0e532919c424d99aecba2a820095b136c2b921ace61b945b1dbb9741" },
                { "th", "0d4d7f6c0e2df9a13000b72abd9d8a7a6c5441dd253e0281bafb4923e1c85c4e35a33b3e87b75934611bb8fa61f1f1c4fe9858198cc508bd0267e435d81fa9a4" },
                { "tl", "7a8845a54b5b64ae1dc31be91d74007474d5604cd1b0871b5c81a8963be33aeae70dda9fbf04fb72256a29fc7023fe10577640c2b51061c0c29202237c152c71" },
                { "tr", "b7468b72e2009a76b268251c5a2cf64b1fdc0f5cad495c7d9544f74c787dd4f164486e3ec503b61a52b5edcd4d5036d844fdfa18356a107f66602d81011a64c9" },
                { "trs", "ecce701c8bbfddf548b4b18efd971440229b53626711af4818f7f7afef9e3cf3d2c3c55cfe74454513d6cb199f94183a885fd7445de330a5e1a0c50f9d98cdd6" },
                { "uk", "007fce2e171e7b3d143b58ab013bfebacd46a37fd120e328f98fc6e615e59972694ad5da9bbfe344e36aa4d9ad1da31d81dc7448cd7add58b9ccfc723be7ac39" },
                { "ur", "74c9e27408e992e374d1dc299c6e74152411cdecbaae4eeb246177aca68156b9b234d77c0a2d79e20506866169157a803f659100748308a28129d83bb1b0a77c" },
                { "uz", "9074a78561dcf797d15bc528a9fad5318a4939563e875e0acb28cb98f68fb54958ca54646af919d6c464d8aadaa84790ab39ac56cf054101164bd63312d8fea8" },
                { "vi", "de9dd5e22ec529a92f0735f27ef6af3be5dfca654d1f951bb6f488afcb3e03aaa02b63e70b532d3a94278868711c1149e7e043908ca61410b7d1c76c5f982761" },
                { "xh", "ef1f918e0acf254c9800541222ea39dd357ca74afe27c34f5130961d343d510c71e975f777208a99ca2ac3f90bdf4c9bfcfed632b37b72a366aa09f74fdd16ff" },
                { "zh-CN", "edad6fb0f827aa321bff9fb0dd1f94c1a344cf91ca5941a8008ba6a8b4fbf033f8eed6f777aa21a2f03955b3fe5e226a15c3b3e21c42b5143f2381ac576e51d7" },
                { "zh-TW", "da8f632e01137f013ca234ba8579116b4cf100f665903c9c8534fc07920964570d272df66f5f302470a97038ccfb14954d57203de74d778d025058ca8094781a" }
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
