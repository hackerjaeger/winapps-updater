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
        private const string currentVersion = "129.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b6/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "916504bc84fd2bd945796ea70605820f5b4c5ec8d071f5aae0f398b31a3c34c18064d763d001ea6df5bf4dd6f8d99ad109618a7ef4dcb1be0af889e927477cbc" },
                { "af", "04ddced8bb39accba696f571ac732d042f86c9b19829d47bb776d66023c51bac5688e0f175cb56d3e20eec50f8541851333fae791128b1f379b1c5f6205641cc" },
                { "an", "6a37ac68551511fa6b25b9d4a348c49552332f2b987a3817ebb665315f7744b1f18a7f9387b79ce89376b681b618f37111ddeade784424ec39211154f17a5f37" },
                { "ar", "40476a60fd739e09c938c99e68d0ba93a2a3039b6836df32fbc99c2acf4af1b1fc22336bd9972a9a14d7990c87ec0da0159e8fcb097890eedd6d77fd986ec01e" },
                { "ast", "3bd7b9dd5b0e04119ca3697bc586cf95b19d768ddadafd000c67c0f2a63b290d62be87411d8f2c0f966998c421bf954e726e58dd369afe2c683afc4efa0bee2b" },
                { "az", "31f7f0f7b2eee41e145cbbefda85f7580e0bc3ec0e8e4a48f94e7d1eb409f6a28561662aa9223d995e77e6b6955362a9f59792980c4650aa0638ac0488a3482d" },
                { "be", "138eed66b8bfacd36ddf903d45e0d2921470abd70eaef14bc49c356ea32765236ff0fe1ace42f706cf790dce326c981d53a7ce2153dc57361edbf9e96245c67b" },
                { "bg", "e2db77c2c77297f27d1fbdc1a4f195e566806850a0279aced381723d8127e0e8ed9b709cb74c573fc0bd6ed0428898e00455173175219ac5531eed1002269c65" },
                { "bn", "74d4c73986581e170d3c8519b772a7c5b449dcf26f71234cc94a101e60178df6f35d1a3af4fbbef0a918020418e7f3aa7dc17ffd84547ee496245a9824b89db2" },
                { "br", "603bd0b107d9229d07a8250a0e13ac1815559112b93627de3df410a7ee550e8c56949cb8d9d7f334d4d3deb5e99c60742f72a94a4633fbad856bd0efa9020c47" },
                { "bs", "c7853bc775fe89a1d8bf2f3e6dc60cb6b8bb1741e01968d402a1f17fe3bd6026e0cbcde687d85df22ed51e20e2bde79b857c8ba251f941a0d39267c7f56bd228" },
                { "ca", "7bcdf426b1d816a5640de2f934226ddc96fe80e6370c141aa2472a768c25326151f0dc6d38216210aa843e66b46a7c7cd5ff767981446ae10dd553157bb6cdd0" },
                { "cak", "2e065d1b08d1e4c00ac6b78ad70561a313308de2a1e9c18e9ae1a7e5f0d4143ef3fd4ec48f8e45a63649d650e2fbe85c702eeed0bf2ed7f82e6f44d025120cbd" },
                { "cs", "8e6f5f5ddb377f8f9ac95da28e954639388fa8613fe147e363e77ac2000543ab54bda92d2f929d0a9e10bd04a03b4d86e3ce079cd638684b96fb94dab7d63132" },
                { "cy", "f34d15f69cde061b4e00babeeaa6f18fa4ce78e82f6b0b099c5e1eb74b0847e69097702548e5a150c389c82a9b9440d3900d5875536896d16bc738ad545909b3" },
                { "da", "7d9bd2b2337aa9f435927cea3ef66c54615441ed829a0b7852ca247d1b189e161947b2c6d78994e1cbebd1beb0ae8ba936b7a47826b0f3b52e5ef0c38384c813" },
                { "de", "8328d244c12f5c6149ae9ef891e019674fcbe50ba0daa56f6295c95c8aea9eef3011a37d7c32bf9640d96ee958258b000d5087dba8b4e3082acf5fddeef1697a" },
                { "dsb", "b55390d1ea3448b14dbc79035db3a0af682339c79e549cb7bc26d61a141b6e93ae7459c20f1096584c70a47e8ebd38c0bc7308f5675a233382fd302ee2bbda1f" },
                { "el", "bdc9fd58dfdfdcadc9aedba2657ab70fbc2c2955877a2b7ee7d3f318e69a7c9d1882f5f722c10fead0729b62a64f07d6a11db231004171e2ab7996ce609ef3a5" },
                { "en-CA", "540a496bb41ff3ce6920ae8b2c03c2fa32fb4fdaf5e5cc28ad0f20823c20562ec5e450d05194323623c9a0569eb83477f0c3f90c4b0bafbedf1bc5b9809d7d1b" },
                { "en-GB", "9f82e8a85c11f8a8e4cdaacfb98e204cfe947a53ddda242b1a572cb9b951cc6fe5162b841ce309d70443466d01ee3fe1cceebff5ecaf1fe0016e768cbf259cae" },
                { "en-US", "35a59d7dc1e57b2143b0b3d89661775dc8133676e598d76e30554a15e6f6137f7071233e0fc5b075a31ce920d15f0e540efe026d5640be72e6e4e7ae3f2561f3" },
                { "eo", "88437c80531f6b902b77d351c375c54de00df1a380a4ee703077b65c2bf3860ec61d8de260733533cc8022ae91381500ea8d599b275e7fb93aeaaeff4fb79a14" },
                { "es-AR", "a4ec6b473cbcb2c392c2544d8c8de6f999a9b54265629fd7e1d5f727da2743444d42463a3ff7ea3bd6349396ba839c4c24ad0e2bc2f8362f43ccfc93bdac1596" },
                { "es-CL", "ea04e0428d75349ac401c9be4a78f0f915bd18051acbaace8a580949efa9454b83443b7c34e0747815383d5b12d71b3427b028a45e6784ce885b2b5306ca809d" },
                { "es-ES", "80bc961910d11c58f9b817727757abdad220053f51d6bea44fdf411276db8251ce9559477f1ed64b859c474ad0c6c07c544b7e704ab0226950f826ddfbbfc13e" },
                { "es-MX", "21bca88fabea1c0a4ced12a30d1cb347274b37c7d8a6ec20d16c14d03e80b90b690fecf8def6384449472b18df1317b3e80ee6b4f9aae471939ccf82be22dccf" },
                { "et", "672f388e9843689cd381b74878d52da6b8f58e1e9ff4742326812b96c31c7edf4555d5c6ce30442e4e743a4e47309ace639cb65b696735153280e7b46c64240d" },
                { "eu", "e22e0bb5de8c572558cb6fff85bc11f7c364a16bfc1267e590c38271f008336ac23d7c3fb334123e911bd68adf8d0f7832b35b4c4e6b9ca0fb63fc6f8135cfd3" },
                { "fa", "953918f6e67cc9bbc6b5f900d8c9c4e6e5ee05b7cbfd3c3d96e9a3fb93418e08a7f7dbdc9e7c8b6bc9130a739892800a52af138663a2d0fd21e9cdf74a07b917" },
                { "ff", "bc803fc474553935067f5dd69a94d65521edbce8841a1b87aeadf79d61c417530e88df635358213d17df78f7270d9b076fb98ec1a5e70eee7150e1b985ffe95c" },
                { "fi", "b76ad9ae15436c651bcb43f225b5f9764a748d3e7b33b5a8bde864489011e6caaebe7ed3018da47a0e3978774f7023f38e966ca5a0429836a65a16b4ad537bbb" },
                { "fr", "3f1797ab23806d35a0e4e84b7677dd43458805037590c924d91b7121a2993a68a1156a6d20d308fc07822f9120376dfb21b8efb8e3d646c1a6bf86d94288c85b" },
                { "fur", "fc56a0906378e1bee49bc808028f8c1e5570c3c62a95da9938beddf7b633d147c3f7b0805e21cf305f5e8a2e710df6f8e7ad155a5b71913f51ac865d5dd2135f" },
                { "fy-NL", "2aaa88604a2190b0f229aa6b34dfeeee2558419ca51846ac681efc2e5e52c19f69da9df48b7c09e0c31e67bbc6feed85873e184709e3f50156f7683525c870ca" },
                { "ga-IE", "ea80ec2f878b6584cca29bc2be6868b43d946c5c352cf54e58ef80f52ebdaa09dbb71a5d8c008c67534d014531a1155d87fee4ec9aa6c0764130ee6d3b61a7b3" },
                { "gd", "231eb812bd25ede627fd075b9bcb34c3da6915c5e7e48b6792e1e5070daa8b399015b04e9a0e601f7f0f85070e8f7da1d100b5362ba5c9292016402d28893972" },
                { "gl", "ff7e83e43627e1c0def3f6d66b221f027fa0a21db0d35248cd0531ff11b1b9c25349fa481a0f47342c390f0bf2922019dfd56274fe0d1e8357d367c2aa4d1275" },
                { "gn", "647b340a7a601a48ce9585e1538dfffb259ed57b643192e3833ec2f0d3856bdcf1b62ca70fd68483afe937f853fb9bfa186ec2315993661586ae54ec69e89262" },
                { "gu-IN", "5a825c638f5cfcb321ee31f3043c7c146053b2d9ef1c7ab33ddb78cd1480d768e6a64f58c59bb4a10e3df335daa5be1c34849e1f8e81bb73d14b8cbe2e8aff54" },
                { "he", "6fbf3dc64b06a45fd8c59ab18731da4c7a86b2b7b993cd60676f2db57a08daa5e2fe46425b43513ed7b273b3f9cfdee50540bd00c4228aed44970d2719f5d755" },
                { "hi-IN", "6cfb30075d04f55fdf108feae90e3dd0fad1fed8bfe1103723de53e1e086381a35f0aaa7bb5e66718b4631e61c3ce6b212582e4ec58d24677b3bc12cb240e20c" },
                { "hr", "99467db4731276a3744746a69d1e3e947dec02b1a200b05c7b6148620b2271235209f1c2169dff028b5a507c784349024785672cc7de3cbc2b2f5e697c753186" },
                { "hsb", "bf1bdb04a39cc4f6da1b63d8e0bd7692834be69bfe407c75cee0482ca52cb71173c9891003b0739414676a885058c2d210795b9677eb8a77f28932d22ebfb404" },
                { "hu", "b1ed98c7ebd60d906836b06f8a31cb5bf406cdaa1419b5752b9a2a3500cf55c4c7009e4930f9c77b9d5a4f6984707a699da5acde05e0f159057648a9dadc2dd7" },
                { "hy-AM", "49f5db89beabc1172c0992b88e27e5ea54425305b984d59cb72b4c2ab4228b80b65dbeffc744c67c201782ed278adf7babdf621d577c6b7366dfbd09d02f1262" },
                { "ia", "5677c68e5d527b0ad6e284028cf159ca380df24e90c25886f9f8015d8af6bc799e28d46061994c7420d6e4124b507a411011ac25eb4bf47d90b1b4257b4a9b7c" },
                { "id", "1cd8caeb447eee65e3d378af4e32757ba2972e6ca9a51dcd9e01195b5a70ee36418b7fb3740802e3b3fe39bfcb0b597851dbbd748690896acbbccecdd323c68c" },
                { "is", "f92584d9fbc8fab64dfd851ef31741cfdf73b038ccd6bb4f3bb1fd4ccd05d21b023aef98f26b6613858a44dc371983ad0a3505f6916a4e11640043270be35a43" },
                { "it", "348d1a40fba63bc1f726e63978edfc5f60b9480af38b93520fdaba09d757417e5652fd49b8b5bb890329c762d3fd7f01bd3cb01b2a307938efcb7b43157108c9" },
                { "ja", "ae74652dc1ec095831797120e45adac05bf9a3796c47335710df9b354e1921e2685c690b5efd3b633f881bccf0c99fcbd276efd920b5670d4a4c56a1ab2c786e" },
                { "ka", "0352df5cc509c80b843cb22fcc5b93a8887bc906cc5618168846c9b6270b630c2ab395bdfb7f6caf0d131fa069eb0956253ae4196c2248c18fc9ecc6968d6fec" },
                { "kab", "fffb73d958823d7f2096881f881038db10150c2412b2f8dd2a4736d6c13b0e4c5e8b3da1ba57ff141337a274895ed3158a9acf9b4f5b4af5d3a0aa025be75d7e" },
                { "kk", "e62123f8c95effb6347ff50bf3072c1a6e0d5208ba46a5eaec8dbddae4b9bfb9fd2dc4437930b439298df8591f81bb35d5fbed9315d9bbbde1e6738dda56bf7e" },
                { "km", "888ad08e9b684d9e10a489f06b5e206d22a475e28d702f730ff5b027d5efe07ed83a9ce044fd3540cf9f91f50883916f265d6f6a59626339f71adfe36c82a3ed" },
                { "kn", "0217311b2bbc0318e91b2034a782e7b4a3c4568723581b46f07509dede26cacbf5a34ce697c226672fa5c75a173b6f05d64be19024a4d9f818ef09c800d58af1" },
                { "ko", "1d9404bc20c9afac479bfe39f0d99a09835a5622bff7e59feb293916748d2bcfaa2e5b86a1a08702efb0a56c4017ee390719f3cfce315dae21b130713cf975ec" },
                { "lij", "1a859b860a641854f9d1e211350809c4e35d76d9dfc1805e1c782bcfb8ce9046e680823b5ffd602c94b0effdd23dc32c695598866c1b26258a806aa3bbcfb960" },
                { "lt", "064cc8351ea83a6d70ef3b75dfd8755ee1743c4b94d878eb807666acc47123ff61562bd891ba31a68de050cd9f0f928fdb4c49a16e53685d3bdf501ed17b271a" },
                { "lv", "283d4dd22f5331ed53abba023c1cc6b3b839bac1a3226c6a014fbd7e99e374de27820004ad20266bdcd13b5ec3ccdb45d735c1c7f501293cf42cc7d2c961de85" },
                { "mk", "f66d5600a0afc914a27003a55dfba83a0613805f222245bd2a96a05e16952c7ebd2f51ba965d5af0ed09c8358890528fee5ccdcf69e23684035ab89504021f43" },
                { "mr", "6eef88f388e43ff58215033c824af4286bd3552c0cf4c416842d837c449785b36800a5f72de0f1beb2d6aacca4fd14ff877f55a10625f9830b48456156de700e" },
                { "ms", "94ae23dab060ac5f266c8034bfa663f103d369746f24decd09acdc9719db6007106a2b1d1caa8deb9e252618c45a4012406d095d7043f352579d14555de4cf20" },
                { "my", "cbdde8ef02055c7d5572b6ef352e32b7fe3b8f96ff35b80d8a03e137b726c97ccb334c1ff34a1a40ab880e0d919cbf01d0bdc46c5257e1c68b52207647c9edd8" },
                { "nb-NO", "5f385efa7d29452ac33151c9f32789e7e728f4fd5f3f704a94c6dfdfc6bf9ffe1f08bb74a00770f7410998557e6a65c73d1cbfb1980c77df7f0dc8b578202e58" },
                { "ne-NP", "dc4e0432f65bcae5be3a966ed6522cccff1a251c56074a8d3a7e25ec76f6ec9af9945f4ae79da17bdc7233fc5eedb48b4c7fd34c3f049e336bd9c0e4dbe33a75" },
                { "nl", "d49f1dd4289f1f4bc93ad94ca0a7b9e313d528291043790a103ca6353cc32f0daaba0ae1b9b748f8f2472fdcaa99dd1d0f7d8b057aee0a0284936df9187178b1" },
                { "nn-NO", "21d7525afdd1bcfd597285ad1f0a1d3c065bb167dea54cc4fc7302023c5329db7c016d1883727f141316b2a4f464086a23f597cc1336dc55803a58319c6812bb" },
                { "oc", "2fe703c564ab63cd90e479b06b054cf123542f94e16d6c1bb764fd12468f2d238387035f1ef27247bfe1c3febdfcd8e3b92d1fa2864149c644a5bc7ae6a64fb2" },
                { "pa-IN", "4739ddbcfe4674d7deb0203103228382f4132dd37c95cdf0da16b93422c9ae3a7707db5c46883e8a4e29f05acc62fea818df78a2975bddab5168613beec8edf6" },
                { "pl", "ed7344186c5365893cfc7c37ffa1f6994b1e8d30b77e6c3fd4cef4831c8109f86e21312573e86b30ef9eb4bd75562a0852b570b5df485140cff2697d95537b25" },
                { "pt-BR", "4e9122faa8e26323f31feae7c43789f8245db6e5a1de347df44251ae0dad093533518592ecf8f9cef1453cfcc44e2bc918f05fa1a20508b147ccf7872e4c216c" },
                { "pt-PT", "f74af85825b63bf3117b0c6f73669e3893e50a7645cf2b03e60b31b6c874238d63f4a300b5a7944ce3e774aa6c8ca25f9cbd1c9ea62667dc2e4260a4137495f6" },
                { "rm", "221f63052d0eb051e5d7d2c1e16dd5aaf2fe0e0c78415fb2127a7595021afb9ba71d599b8591391acc09239fff734b3b44b5f914d2e2ada0a6e18e4e8d57505a" },
                { "ro", "da5713df2676de8982dd060b708458ca7762683a8cbfd34634b0001ddcebf96cec69a9f2c7a2821dfcb511253f2c83982fba31fa99310ee602d1b0aba6b09e39" },
                { "ru", "a6923d48b451756838fcb7366c1c4bf6a94a187e86ff5bfeb3b79fcf81d5a3994ff73ecf53e2fcde079b00df163b575d4ee0f19d1d4d8843ad60f6daafe7c7d7" },
                { "sat", "08739ad253e56727b6cd7a2cb76c084beac1e487cbf787f7f595031b072c59385d90acafa26a26f68a0d2a1af6a96973aa28bb7343bd10de3fa5bb15a40ff21d" },
                { "sc", "a43eabe8bc51940cf4225619e3080f640cb7ef2ec1007002549095cc524d7f764067777e57a749d9d9140b332cf5022caab21dca65ff9ec520ebb4396cd0bb71" },
                { "sco", "5f2b25066c3d415f1241d61ba4e2e8ae3d7efed768525b7fe8cfef9c5c764a9e9f94e6dd392b6e62a1b9f1ebb05dd8d4cac84664d05c9a482a2720e15a52c808" },
                { "si", "6f76602bd7c5a3723bab28da5b6f16c02e1eb21bcb5e496ec81352ea2c2f809232ec4e88d159225523421582bac8c875f352c5831d9e0e65509a7658decb65fa" },
                { "sk", "2a3d5f48378d83d91f377a06f66e8f026fea7e0d36dbecc729fce52125daa4849e04f4752bffe8f9e4f755f3436d03c0d30c0eb683fd1c4733bfc460e1027083" },
                { "skr", "e5e15b3a2c636b6d9f4ba4742c10534e28a100e809fd572f9fb5b494383a6df99e8414f1ae3f9981e63d54fc54b695d678e4711e6d5b1142934433f3f53d78ef" },
                { "sl", "60f8205ba5394b621f7ab4f95f5875c12f99386eed5ccaa9925f0be084432ce89f7afd8c50825243ce0052476ea4457c95527e9f68a81a94d08f9932747ff248" },
                { "son", "872fc68a61b1e9cec8cae60563e09c7727c6c9e034d156655ab01038593c3f0e8134c97372e2fa150554a2b69c5134e59250204f5427dbf3e1fcce27c019d0ba" },
                { "sq", "84f520461246a24d5f4ac21a619c31bb7ef9798ac37a9255e94c0e09bb39baae2a934da56334e0a380283dafe34cc75a1cfd502ba6864ef2bbee1310265f6803" },
                { "sr", "e32d769321e0b7c91d7c10b556c381f8c2c9c05bf0b5652ce5d43a471a1fcafa25c0f91146e720a0389184486f4653be9d871ea046bf1ab3a5e451a89c95ed9e" },
                { "sv-SE", "be4cc0925664f1fae4c96eaccaebee1d4109e8beaca1ae9972925a665708e0186698c74605648f79220be21da8fc042ba07ad477ee9823078b7e2cfd12c8d7f6" },
                { "szl", "d38ddbad8e47377395c31b49b710a6979ace866307127a3e45ac5450f4eae9f413ba5322390322cb17f38de69841169c80a55f5f5908b6839dd6b93616bd9326" },
                { "ta", "09dc23cedd084219ddc519c42e6d5bde8398e6ee454cf2dd254b8031f052532705ae95ee70cd73030d03b998f951394be54f8e7a1bf827d765efbb27edc1e5e8" },
                { "te", "795cc52fd0708b23542f95c8053c1645bd64242deb47247296d288bd5847e39cebabebc8488c4e26de40c4e4b9a9230fcc88d12013a435160e540778edede485" },
                { "tg", "b23e9f29ce984856496c7d05f61f9b77951cc58a6de9174f52109afda2b7a6c7f98ad8d49cf9057ca5d987b2c0f899fa4b4c7c9eef2cdf8fdd91193ee012b4c9" },
                { "th", "b5097f6b8b455d0bb1e5b469ba2d5837e883b6fd755b51ae13272685e0f476562769094d6ded8804d43c7926decfbf21074e78588b29513fb3f3348242f61b76" },
                { "tl", "dbaccbec5554770ff58ccd411c1da7a3f7c8eb66971654f8dc953cac0aae98c45de27708b665814094082963a6fb5ae2381de4aa010fb54a8c0216f6ab3984a5" },
                { "tr", "64280ab131a7be7c5f169b9d7faba2284334b6d1958c756f53c4f0cd9a67913347dbc44b549b8193336f602f1c69f137c9efd3194809c8cb90a4dda6524b0820" },
                { "trs", "1f503501d1648a0c696ab7aa5a2f00092980249e8a1126acd11f30adc97ceec9400957c75e3aea15b6f87c465b99b48b2fc3432c5ebfffa5194197396e5ab7b0" },
                { "uk", "85215a0799fa7b845fa34de7fab56246e21808684bba6bc71dc2ed225f800e619b1214d8aa956e226449424be984da6fd4c3e0553ce2b50e6ea4771fd8452d23" },
                { "ur", "b5823387b656be5f7f803ffe80b2ee1c17d481a8836f52aa55353a59874b6c1ca11ca36575276f1c468d4084ca4696a670da1c4ee20fe07f8f388abf1dda3644" },
                { "uz", "731d96867d19ef9a055a7c09d8329752c1cc850e65d3eb4def0a0c0bc5cd1318c18f9f6d743adb29d13287da1b58fe3586afaed293638d2c72900d2f1502bd6d" },
                { "vi", "617c9988d77b9277342317fa1f66df2049cefa6485db5c05c40bc2d2fca03e4f4062a23db9c07c0129410bb93e3a1f0b8160be82327c62b22b802a99f7abfae6" },
                { "xh", "a1710b917908ac1b3a44064f9b08e6134e6359e9deee11b62b3255dcd29720a296e8f858c1d0f968b8d41a9dbc2e4e70d0d7250582d746c7d773d2772cfbcef3" },
                { "zh-CN", "f17d4c7609a8ef2ae29ebb113c75b2b77dd868bb511cd2e2a1394f2ddad760e2c60bf5f1ebe0bfd7777c5360ec6a14a95d1e6387371d6d5c6535c0a0f73385cc" },
                { "zh-TW", "7fef6554d764486cb54191eed632d6656053650a02dcd6304011262169c45f9b2e9a0597b4ab944c79c0460546de76ffd7a10c93cbba045a4c7546da46916d5a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b6/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "201e6856ae0cd33362ef971776b4c66845fe5902a19262d67c20a9b5c7648cd076066df4f9b80933d7e1079cebbac7887b842ae93b9f20a1717d08269f91238c" },
                { "af", "f3ed9c84799309876ae8ba291c7312b51ecac6b36afe0e97b41dbe23631b1f83aaf7c6382e72a9b9c95c01bd3d9aa9f7129aaaabbc01603bfdc18ec79ca59c3b" },
                { "an", "897a5ccdc4f3309f37b65343652a103ebbaf3a71766e19083be942e3d39223047f022c401bc56ca1606c62af0f3fd733c8c1736ed6e1636bb7191146de14547a" },
                { "ar", "a92098c08f1e1b07094b24072dcebefde015da2cee1a26258bf10125232627c4529da9e3112b73d46a77a99c82b134d39f9a323351820298ab927f54233ff974" },
                { "ast", "41a45d44ad42613d1d328e77cf8b8a83ea7373408de4160d3b80e8c9b5301c2d552c9d8af59ac3da2ae3a2c0d5a555acbc067cbf96e16ac6605f4e342bd9f79e" },
                { "az", "ce038ce67fdaaf8bda08bc6dfb3f2e5ad88c3c243e2aa044fe4a7e9b104950e5ecfb603a5c1dd284502dc3e8d157438b49fcad2f560ba778c3ce712b241df5bb" },
                { "be", "60e42dcd283b4f5da10e0d33083274baf8a32d345cb8d8b64572605fd321c180f833695c1dcda002df18f0835eca98231c571aa7c3e38370b852de57246503e0" },
                { "bg", "6a5930f53dea119a4000a78fe8f7b5fb096ecaca4c68b52a146d75deaf04255e5c198f132242d2cd9c65f66369f40cb2b0e5656cc4fd1d170d04fe3ef9e4c16c" },
                { "bn", "c273443457f042a4a687cca8d623bce0e440329f9afce23c1a85de87ef88d1158002d11fed4ae44e1026e40f88babe37a188613681df85920c48fed068109630" },
                { "br", "a09c86204ae7473c1e6b14b6f02032a1ad86320f822878177e3edec6e7fc3a330a9adbd892679dd07cf8ae8d95c9b94f089c653935ecfba6149d6e402c4e0a8a" },
                { "bs", "f679dfa63b0a912fa37cab0e8a5cc3a2cd9f8b5c47b5c216f18999b3803ba8e2121a9a613a707c0c04d9acb66061c11663e0fb2c366909caaff297d98029a8d1" },
                { "ca", "5e7815704d6f6f346dffbefbe33a8d81a350c86d5c7a65b8dccee1fd582a66e849ff1f7b1ef1fd60fbde3604878c0f14557c849538700e5002f22e4327790c54" },
                { "cak", "e758691833ad86b253ed0cdacb4acf255872ecf8c8068b39aca82ad69f43bb7bdf9e8200bf12f8c0ebfc83ec13de93fcb23bdad94f9eda91d87056176c1d3235" },
                { "cs", "2dcb62b2fec3a472a2cace5a2336a6257a78478247f2e6b2a7f0c4e3382c0afe16a3d9329ff6799d3607921190cb974203f25c91c325058376a5abe0ba9e508d" },
                { "cy", "25dbfe4698fc13e3f84d1d556d302e0138fed88f81e3fd2dbeb1f6abeb16b3839b2cd94836cbfa7da6f5c7c2807d3211faa98dd4c1ff13afd61290a591283fe6" },
                { "da", "679af79e085c7afd457e58bf5561850c7fd488502b29e3ec3d157da3c55249bc13ef91a141437494353583fa9645841259732022b0a9742a93d270aedd8d280c" },
                { "de", "956537930a1d30c600dae9f8ff3dd719f202d8067103b7436fee796cbbf1226a076475505545d1402ad5f3481f4d97a2bd00adb5c80fe19c6c462b27c3572e6e" },
                { "dsb", "3dce49f0bac824177c5a47d618009104d075d37fbf5d713c57aad1ef6193d573b9c07d01d6c28c157c4e857dfe8977cf502cfbe3ec0c52bcc6e84d1a809edd28" },
                { "el", "d1cea41846f86a12134cd7962fc7d85a5de7d5c474906d80a6d4b695e84a783618ef5f0893c681c215ee259136b7bc3f3067ddd59c3c1f40cf0cff16c4f9ab68" },
                { "en-CA", "f12e5b18c65cc6eb13cab7f176baf748bca3de79552ef6bb6eeceffdde3568c17d93c2aff8a878e7925df347ba23a3486967419db0df2e0a1de47555c0691422" },
                { "en-GB", "bb367f1d5409a513e07be66b3fda28b7a8b5c98208da0bd48f86fe65ac1ef9a7a00d284d353843aba57e39aac6db945165984de4b1cdc6bf77958b9175edca03" },
                { "en-US", "50c418b77c31c281d82699bd1c8491430690465a85d1afa1b1958e68060cae1aec0f35231dd0af1e09d6102160d6113b7f4e235b427a60a11017a6015bafa5bc" },
                { "eo", "1e6d7f1a3b066df2c967e4e07724fdf307ae26a9d884ef6220c839c69c83a2b1ebdf74deef7c93d4685de18050a2ce15288716d7362dcf44b623d0ede74ca36c" },
                { "es-AR", "f936a1818557b229589315bda806e9f9e2319c80db791bdf004379322ba895993752f100bd6605b76097be3680bd5136e2f98edb02006dc06484a311aeef8173" },
                { "es-CL", "99e6e5420a496b8a75fc143c78c5f80a11a7dbbcbc5c05fe6df3297846346e796175888d947715e5891cafac18b480480625860ff187894a793836e432b56be9" },
                { "es-ES", "bf3857e03db36fe7354b9271fcec33df85900894b5d32b831d73f45fa5a2713a5de076b72573890f537426d47c6a2e608e50e1522a933a9fd7cca9ffcfc7f004" },
                { "es-MX", "81e009a706b333e5f1804b9e0e68d08747ea75f16fea585943e10eebcaf4e1b5abd40b792aaffc45da38b7afc686ba36797b84bb8a0d9512024ecb970ec33511" },
                { "et", "7fefa8f70ebf9bde1e3e527381dd5182ff7fcd372fd9cbb49268bbcff9a4cf6239d6ab9bc329a6bdce57625b2740a1bd1b07597a8b96a036e55ded0d4e673637" },
                { "eu", "c5b64cfde041dbd5962a47859555885f277b36dad13796c9d14e37b71c5a8dc3ef54d7ead66b4f299dc6b3df13a9117f7bb4882123b148757458917f0be0ac79" },
                { "fa", "b3e93d3f4b8c8f5c39f34d37a4212146506a256f4d0d85ae5aff3d5d4baee5fb8a508cde9bdd73a376ec6cede4b0739b3224363115d5222b6fde2e7f8d5f7b72" },
                { "ff", "16bb32e26266383533ff2077c45127771b09992e17f258a05048b7401f9a548e6965352f423d7cc219362e102af9b911ee865a3d9186ab6914f2a64194965457" },
                { "fi", "55bc4ab13d627f50a575326d49c41da2b775bf15025c5c55b0e85c2e85d182059e5a96af39c0f18d559003fccd55b497e2e1891b9d6e64bd3f8a74bb19c627ed" },
                { "fr", "11c2ac81e5a42d7a190d930390aa23a135ae46109a658a400bd209dc77336b3bae15a8aa3f5bf5e0324f50610b6a1c82afe81fe54371d17d00f8ee72c6d662bc" },
                { "fur", "bf0934a2f12a3b2b754ddc89d611cb0c6198428c5f1ffc6f221c569a1c87308af7953897eeff1c5d1a972b36d8698042515be2a83c35cccd1201b01166e3443d" },
                { "fy-NL", "10554ea6f33a218b1a4d70ec07a3d993b0738c390505c8960587a0d68cae4475e84d084f185ea258f37d523c48f7a24c1c409bff1370414c78416e3ad9bc8d5f" },
                { "ga-IE", "c25b32133b684584e3c336edcb05be04f1f481d243a15cbd15a357f1376f70ce2e4e96e796eeee0c20aeb1ee9901e8468a67765c30617c2adf40365aaecc589f" },
                { "gd", "ee762488e902d45837b5c054c729096f9bc8985c731cf5fda45b6bfa07fc7de082387428a24d25600d043db333d3cb99983ce78f24a1ed5c5e80489e988c5739" },
                { "gl", "961acce3fe08f0fd5a136cd92b2ceb81beaf493fb163b2b01178c4c6c9db7c6ee8fa4e17085e3c4f0f824200540127a756a3c8ad3e584ff4877f233ef064d2cb" },
                { "gn", "c4e18c7e75166f917bec2c742b06b2281b4b2ecadfa6c62860c3a4d1bab2e7050df5d7454cf21fa519f0f4826285700f9358247240d1b24d44654c1a79ad0e85" },
                { "gu-IN", "c3da4fde0c803a0d71c7be53d95c0a84bc44f95113ecca0b74de45927f468a87907d539b29e1cec6fafa3d3ba538b3fdb4bb0fd667cd0e70cf058357277afe63" },
                { "he", "63efd4b4763e6925b28c22f4f6e1ad03d4b75e4d4e6a8ed2ac32c1e67108f404159314726cc1404c83dbafbf8f08c3d7ec2c84c2929ef5e4a93d997287143c26" },
                { "hi-IN", "3fb24c9c6e362665c1a4513d90955d276efea5d0edeed18d5862feec2f7e724ae7573d8d5e0724ac572cf57279f37820c538b948707d875ad190dc170ba99a32" },
                { "hr", "48b624a66a909239308670290ed67341f9906d468e2e0adac2dc86a26ab043751dfb29036781486b6f76e5725f5be458c06af4367e18ceab0789f86db75c2f3b" },
                { "hsb", "6c8cc360e95e3ff358baa5e8560b67a3033cf25e7d45b71d56f8c531a8a54c97ca8cba3417c7378b212c8f99bf11d01967f41f991e94d6f8d5bbd9a441fa8c81" },
                { "hu", "0d1d3906b8620597aff9f93f431e024786715a499b508f3c1d13806aa2c55471de154146741c49a3562ea7c4e5405513183ec6a56e01e9b8ef8bc524450cd692" },
                { "hy-AM", "b4d5ea0c0a205d4b0712d2e0a0eb1c628fc1b4eb6c6b18fc141cbdb888ecd42b96fdae972a31435b3e778d484e39b564f5d1f11aed8bf499208501bd54356585" },
                { "ia", "048ec288369995af4eac77d0b363d99fb92a97aaa27e0bd4987d2af2d39d6c664ac4cadfd9fee3edcdba8596820655753de94e330bcab4b5770331653e170c52" },
                { "id", "73fb6a82bafbfa0dbc6d06687cce39d5c7cfce3d3dd412ce98d6d1335ac0ef7e3e8a112ae56f63ae59a6c0b8fd3a0ebb2e196fe79b0f7e1dc615efba0f35897b" },
                { "is", "02a1a57f53565eb52f186274d71dbb5f5aae43462907cce11d94515bfd3be4e182b10fb27faadecce9cc08e4a837d7794c579321a6c22b946d028f9996efd8d1" },
                { "it", "c28dd7e1b3da0cba051370e478b7cabf59d7bd1e33d3adca88496af366304260713fe8640d78d12e58622c85285bb0ca0f42861b863850fee9a688e95277d386" },
                { "ja", "0a32f8117003037df7014528a3d1b6bc15d50753de78091c00ad153b8ccdafbcd5534e95edbdb9fa70968225b101b3487779248227c4b68e23aa053fb5d43fb5" },
                { "ka", "ae643ca9fad7a6ae9a6bb80046ee3f0800a5efd01797eb8f021d86ef20da5cbd02ba36ed639407bd6205233e9016fd5f443808d5a90f0e67d92168c3a8fbd155" },
                { "kab", "6d425673c2d1ee7fe9e951d00cbc9c1ba14728edc669b835b497467ff9d9ecd69b7d3d0d370dd1c2051c66d191896ec033a3cc2f10c35946e911553bea3f3918" },
                { "kk", "33db2597f6a9a113b1f219c6241c1c052040e9b1654733be78f9715cfbc22e3f251ce017093391c0acd7b0e6f2508e49ecbfdbc5d38ed1bca492388b84bcea01" },
                { "km", "5393c463d98bdeefc39e51142d37450b4097ef95a88b262289ac09d66b16ae10786e5dcebb2e48f6a50b24562454958aff07818aaa37a756afba5d430714b0de" },
                { "kn", "447dfd4eaf303ce9f6fdd4bf8c1ccea9ee93c760bdd0e88c2069b6377304ca4ef8ead6bf2c739128c73ad4cff909a0b12cc8679602549e5eea3764292c2974ab" },
                { "ko", "7f3e8a2a7eba4326fc938ceebb15acaed146b27211dcd584bb18dc0608706ddf4bfe1cea6cb8f3115136d5c75b76a87b4235cd7d6b7beb7f8c8726cf6e937ccd" },
                { "lij", "5b14634b86ffa91cf09595d4229441500758405c18462aca1abf9abf8607a2580c0bfc6f91339178660540f9814ea24ab0f795ad2f421f449d9c28a79f712406" },
                { "lt", "c03accb9f0b8232f71ef664a011df35c760808d1be60fd2c38ccc77415f29c0237d429e6a57dcd75f535b0308726b94729773e2d13a75eac1fb58cfb4175c735" },
                { "lv", "1ce30876479093f41061c96a26e2734efb41bd837dc32f75b9230f70ffb9306ae5545450c5f2bac675d49dbbcdb1c3f0a32911fd934d92a036f5cfd6c65969bf" },
                { "mk", "9098cfc53df8d7df5160faf53c3aebe85cfb4c17d14fdb4a364c8b4ed7c0a88de184c9d56cc785ab417b4892a15d4ae7d5080c1a895af3c34d186c880ee11bcb" },
                { "mr", "e9f159778ff6e5dd6a93f0106a7d1af53b65d0c59066272209d00bed48ed15214b641c15e5127b57fa4967b21c1cd597fb10bdd206122f57f97507a333dbc92e" },
                { "ms", "50b0e42df5b9e5ada68a91e8e205d17269fdfc700ea60d1ff3ae895c5896d1d94bfc963d9680587b6278c3cea4569498a455e04f0cf655518e8a8ac0341d45a7" },
                { "my", "8d88ada414eb45d9d28b8a3867d360b6c7c54292b2143192b5d2c28c431f0930e1a4072b8ee7164b4477f10e360f88e386226264f5bcf2b7976676ce172086e3" },
                { "nb-NO", "7f076bc77a8e57292b915704745d8269f49a1369dfa77c9c9dd49733400b1663a76c741a988ff50e4f088d1d40c1714fe7a093209507624f003dbbaf52f1a5af" },
                { "ne-NP", "4a08c66fbb46594787dcb44122e7655f811be8aca60255a3099e6170690deeaa11b44ae5b6cc47cdbbff162e8376b476fa42562b20cb1d3a20c2c04f74e462fd" },
                { "nl", "1ed253d4109b3e0162cf4cf95b68c46d68b6ae7fadc5a78ebcb45c9f876c5bcaff5ff2218b5e9e86e8ef440c3d8faf77d54e2d920777e560f3cebdf9fafe46dd" },
                { "nn-NO", "1c2fb503b643a95ff73174ed77a3dfbb2da8aa93a0b6a85900c427ddc82be28c2ca11f801ceb74fafd4085caabd7ad8e7591b54fe8b1f1962c72fa1845a9db9b" },
                { "oc", "63520270988365cc3b37893870c14b4d2b146191913781a51261db03ded9564578b2610cf997a75f0ae95bf3cabac8ea31e137b97731a8cd3efed1d0bac346ec" },
                { "pa-IN", "44762774626dd67989f1c905d70aa2a990a50d280bd4ebed1f3d00b63392f1538d3087c461fb6782eade03c84c07c10c69ccbf9a6432765025693653dd1f31cb" },
                { "pl", "ff7b9e7534490be11a28963a807c22beac030652ada21142e7c31e074d0e58e12a225957ece20413d2309b4c5c600cc123df97228e2359e01e7692c7e5cebf2b" },
                { "pt-BR", "969138eadf201018e9f2de073095d3166ddc42d0407a4b34bb67d8f9e95b67595ec5515104f12b2222ea65ff7f51eb25ae5f94bbe3ebf09e383815dc8f9d29de" },
                { "pt-PT", "f810a3109a0862314483fec7464400ca79748c0fbef3db61784fb436d47f5c4ba257c010580058f84c6677d9828b618e00164f042148b6c2299e4595ae609622" },
                { "rm", "c6bd556a7271618ccffcbc5a7ab29d9650b5974de261f72224ec08b82821ba5fdace672dba77527bc3eedcb242ad84fc89b2485ea27c5a04fdbb136221ca2f6d" },
                { "ro", "1ed1742fdaa65928ab232d498caf5d591e690dc893cffd5f0893fe50b6d0dafae0e19b86c8723af2d47b46a54faadb0af168d87480410d65fc6dda989f8b94de" },
                { "ru", "0e785f1a190d01f9b575d5f2326759d3922a2067746c824e4ea05bf243397a5ea8351cc2f4c018a573aca3099f82689e6241f4fcc34b03db1a068d630af20439" },
                { "sat", "b29b5f942cbac1ddddac8d09ab0e10044255d54cb6f611fc7168f27dcd7935012bcf61bf873ecfd4284c5d7768f0fb573e637fe465273ef68a18442465fb21fc" },
                { "sc", "bd41bac1612e64c5653aee388a99f97d7e61510ba7dc7977187976932788a9f34b159095663ec74c71b540a3bcf2e8eb4bd6bf4a81493dce4265cc8d8d617418" },
                { "sco", "737be0a143e6c276c3bf8c9ab99c16a7a364c463680cb12180998af9056ecc1d3b98434b622888b78d15438a93550730f9c637fde142041ee53382133bf75444" },
                { "si", "9094f51657f6d7dacfaf116cc7e900ec90f614f851e0a38e481f9abd55a03922f96a7d0b786fef6a07f9e2bab4179e94b8be168a46b02155c9fd3d069d12577d" },
                { "sk", "92749532f4b3e55bdff6525f2d8961ad2601a3a3d8f6fd516d6e1b2972c1f876d03361ebb9e05e0ca51057320d0e8bec22a208f4522810894ad759665ebec1b3" },
                { "skr", "91122dbbe6f87f6a8a54a16679030261bb0c32b94a6dd44f249bca5ebfd0a837b316c3f206d315d0ffd3738fbd11b511b054f39f576196876481941e7ae19b35" },
                { "sl", "faaac01a8237ecc4127fc6ec9b6de4b6f6d1feb3e18a82e4e06f9a5c60517c77e497b4e2585d950b1d27e0dbf449db70f664e8821409bf8f52fbfc7e6e3ec9b8" },
                { "son", "6f89bfda49612ec1bae8487cffe5872e704e79ff7bd04fbb9037291de5c74e827c36ef277a8c486aa8cec2f29f59233377e11e81ec5fc07a3df62b9049874cbe" },
                { "sq", "b224a9480704cad19a60bf26f21c8374788f87dec03b3644be52fb68f0ae2cf2d8de2dfdd8f4f24b5563922e6ce4544d46975e02aaf826735c38c7257fc18f53" },
                { "sr", "703c5e3b905b6dcfb9b0278e5f27cee983e5d08288c8411a9cbe80db9539d9b1853333db77a08a1182f15d043e9f19e0e816b871c10f0b21c3ce856113bf9d13" },
                { "sv-SE", "e60588acdbb2e7b056e45eb1c6ba83344831dbb4efccbb02a00786e5de5f462150551a7121f811cc68dadb2ee41f63e9ca9640432329cd1a3b256193b4e0a4c6" },
                { "szl", "4f2434efb88d873f69c2c60b5d95776bb2040b875ce670b6800ade77cc041222e4fdf4b23578c7709aad8a5dd61a26f42d908f4199e2de0d7ff4c12548bf386e" },
                { "ta", "b3ebd533e1b7786bf09c8e70d947ffc2ff3b1a3aefab0064643c6c288b8401d0f4f3bcd0bfa0615c10fbba280b99b5ec284c5aa599775b1c1c1e73d4fa16fe82" },
                { "te", "5032764018e19718a562f25bc8957ea70ba7d0fde224e50ab265fe93a0563b42ba0c47d2c63b5442956dd7dfedca30ab6b04c24b371985b815a92d0746ee6b37" },
                { "tg", "6d5eada6d6bbcdc0fbc78b227d80810a00d0832013f6c4fe92bda512b40aef0f9eab8dd17f8bde477dbe7a8f8e14b6ba34496b48fc0d43887cc5aad9d7e03b3d" },
                { "th", "7c11e131a472d918591da3369c9839756ee558e3abbb31a6511022a3c4c73cb89261f07a614f74bf9252a029871cca40686cddddc3cc543a1134bafab4a59d4f" },
                { "tl", "b6cefe0adeb5aeb05652074dfa80d1299acf44aaa08b74e2534ef86e6912b8e2b73d62d35f622c8937f443f745218daa160db32d08cafdb38d9ceefe3ed20471" },
                { "tr", "dc63dcf1d4ab576b0b5623d0a3267e59ae54aa46880a2a6b55582e813c04f0f6a95bf7878d2f1f94ff8e943ba7aea0beef97b2d610144dc508e98f1665fed338" },
                { "trs", "51ff4dc862a3e158942ca83dabf7f5f070c00e554937a185631d9bd034ca5087fcd669c991fa209087fe152f5e1d6bb673b48de40ed1ef8b6b4c53d1701d2668" },
                { "uk", "ce79d90e29df7aa6d4d18ae225fb29528e59202d50c0a75778591a87d3c4e0e9fb9c19ab091ed1f07786f52b665168a1f6632de60fa66bf5e649623a48ba26c7" },
                { "ur", "62d7f262ce559dbfd928c0778c164db92c763c9f6b9a03a29a538cc0f0d08ced2e76a497850e8973af7d4fdd179b6ab0f281e877ae7bf6fa09f7877a2e9c37ec" },
                { "uz", "d335b53abc44ea98cd4dd6e9d624b41dbc1faea69083d22a6849dd9d7a103dfadfde16dd47b51ff23139a6d834840174b84d0b084b9ea5380fb0d38ea6000a40" },
                { "vi", "e005952146c379f3f2fd7b9601003ed432c13b5e1b62784d76ecb57057be0c27a0672d5ea10d2a9a77c4592a4bb790d0be3cc9133fb772c5fd15ae59f09f7833" },
                { "xh", "68a4d81796762878fba16f8e5702e6442b8b02bf9209325df5ca15482c3389bd6ad76de713b00f222fb2c467ce69a21de9961ac2d304365145d220b3e9aae67e" },
                { "zh-CN", "85ca32ca134a41510a7f1e0e85feb574fb5d86f009df4babafd986232e3b2456617858dd2006d39aa70c7502d0ab2b26b24466dc60252718a2a04acd6d75eae7" },
                { "zh-TW", "03e822422c555592f1ab285d5b5efd57767fb0f0aba8924f071bcde0eeaa60fa481737732ed3c3c6041daf19a3a5e1440aa9f81a609496c5bf81d728ca580fae" }
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
