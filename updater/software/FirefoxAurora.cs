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
        private const string currentVersion = "107.0b2";

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
            if (!validCodes.Contains<string>(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/107.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "dd6869cd7be6d6ff183f9621767cc7dd0fcb06d62393f5d74d90db01c5d92a2a553afb2b89866a69b7fddac517b22f6b6022d4f08dfb23fd4169d4a9a92b53e0" },
                { "af", "bf1d9f8690c69147165e1ee3887ef9407bbfd3aab7ecd4f1f7a0ab0860ae0732c635c90b29d824371809713a6741f0406d91fdd3d0843b18a1735c796b061c53" },
                { "an", "a2139825d894e906a0e6db33be94f684a9efc49fc449c934471189d89ee8ef63dd312c43093a43b6a964991789d13b07c97cf1972f94033d43e541704780d05f" },
                { "ar", "1dcef0c001178cd18a0a30617424db00bfccd69afd81fba69f74f83d1269b37d1a1a6cd2cab2ed0c70aee2ff15f6203538db47ac0907ea6547c990fc7b0d83fe" },
                { "ast", "a0c7b58c5520031fb7493e2d15c931af5c10a96dfda16463f4606b7af3e8232b010c7c9e63911cbbe4bccf9274b17de42085796944deba56b19d8dae9ca2f7d5" },
                { "az", "b6586c1ac8d478babf93bb5cbd893ad416c2e7f48bb19261d6167d00ca0cb3291d6479f0d60e04fd98d520e2f715a7127b55ba002a0753028e2435ae4b651230" },
                { "be", "1f71e2960e92a79f6a563022b9a28364ed552d1cfe35d96c97fede3964fd3ea9d1c5d743d44f16b82dd2575545203a1a5663dda49083db84b2e9b35c5fbc92a0" },
                { "bg", "57823d5dd683d3e23324acd0b5f443e8252dcd895e0168b4f0241d6fb165904ab3e446f6b311f0d736760e8107ce096ec5b22e452728e0645238bdd8b30799ba" },
                { "bn", "65a6b63a102ed1f1ce80e7eaef84349ea519b219d2f6cea0d089f706702474eec83da83b20ae58e55364af88b3c84272fa710c8504dbfadba230980fd6756806" },
                { "br", "13b8a8571f5ed2ce589809cfd4fc2a94e05194d2bf181241167cb43e4b6d53e4696bd883cdd5a74293b025b385367dfb6551e1001b5b557ee809b5b945ef0e21" },
                { "bs", "21be938a61b1c2fecd115947b64df94abecda53923f3333d5a78372347e08bf4b43da366d5015af1f7bbc64f2a51638b2570758aec679e30e3a267dab2eb184c" },
                { "ca", "81a02777002fc40663a83be665f2e64603fdb334a74ea63595c3e07063546c4a3af10e457ed48599a7acea0953f72d89d3428d456afb53a6fe28215b76f27877" },
                { "cak", "0a1817d6bd20df0be28357a531acc7eb1392c223a7046dc7064e6a4a58a3f5317ed5f2e441b1cb64d5aeee7c798901c9043b6a3fb686a16ff4881e30f9da7408" },
                { "cs", "6467fba24cfb4fb4b068d1ab482c2e8b638956f700fa44ca5aae9735f7de06b18a1b9cbc2c62a5867c9eeb12f4650563c2078a2f3641d9a96e0283eac00bc377" },
                { "cy", "681eb5fccf1bde46120061bb4f0c8b43fefae723b2c0cdca87ef043a919724b0c61124e2ebb82497d51a79dc1a51570db17757140b8852f4ba078d4461bb0e41" },
                { "da", "601e9f9e9f32254d543ff2c464c16b5be451056d601b1fc6e580b7554e8ac98225e9b7cc5e273fc228240720611f2645c428c9a358c653588f2c1d6ffd612d5c" },
                { "de", "e0dae2a4b8a34949757e82c6842ba65095cc3e0252a99f9e55b02c0c8f1b85ae7436305a82e565aaa88172260af0ae0b08f5bbc1a310957150dee11240f5643a" },
                { "dsb", "5c54d7756e08840069230f1b5d74ea408348287671f36d2a9dad0b6a1a402cced4a7da2021ef040684e77d73c5a0c168436bf1a9815539329e1d1ec802c374df" },
                { "el", "1bbd9bf71e6acc89ab3683ab9c60231a37a99da654fe2add7e25064014f7bc17c48c8a30371ca0f78baff77e083cb5c0485e62950f0993a18ad3f1b964ce338c" },
                { "en-CA", "d5c157db5fbddf2e0bbd6fdeb158fa5d49a1679599016056e23b63b73434191457ce9dd7cd6a3b2f7a15a323c800b55d01faac536095f3430ac348511aca19fd" },
                { "en-GB", "f14a4d14b4be2a2fe58160473442d63bb18b4bb5492e9bf4ca2db4939bd408864442606fc1f66eccd7abf2642f639a6904850b739068b96ee3f343404094235c" },
                { "en-US", "1f49cae7b0a56fa20c7a591bbb8303d250c4986bd8fe8bc9d62662855be200d321ce60ca29d630c1a4c123fd29c49f84ec3ebf99f3f291c2fc13fa7c784ac3e3" },
                { "eo", "b809c10dc1ece6df62aed685a112d8cd6a1c842120b59083f1b4e9b1b58c01a4c792c2089565c72b708f4a1e8a81c71ebf19cb774bbc7cccef82d37e50f589d0" },
                { "es-AR", "cf022cdea7b634a7b7cfdd871c09623d8f9a3112dbdc2bddaeb4e41223a3449d7628e9dff1d5944d7b29ed032cf9b766abf4079ac0d3b565bda17b92d0f54c5f" },
                { "es-CL", "8e94b6d060019989b719b9be32d76450b57f429de24005ebcd0204809a4f6bc7fd15b1a06974d18430c84b3bd175c9cc239fe49fe647dcfbb97f73f4c1a118a6" },
                { "es-ES", "323cabdef466934cb79ce6848782032b31d9808b10495f6c0277cb53e62803ede70a3c292c3dc6f99074029def126d8bd37c7a1900b9b6a879de40bd00ec50cc" },
                { "es-MX", "21add718cb1b38698c8473fd342b643df85fabbd051c1c68b12c98542ba656003f88e708d2aa9f9ec869d2741b68f577f334af41631734660deab9b62ed1ecd3" },
                { "et", "b85c2ada5c03ec8379f60027e5ebde6ae246e06f92a6f53cfb723a12d5d246dfd11b2dbc7e889e17098d2590c5114ee7a14980e4619be4de147be1a9dbfb4f96" },
                { "eu", "a98b0f1e523a4630530a10cdc9af8890646f03e7e753f4a2df65d984c9aa05550d2692f34a7bdf6a94f2a8aeb7088f1411425880feb8a96b83a931ceba39edc7" },
                { "fa", "814f7ad945ac6ef5986b2f087dac5fa6e99fa51f52ca2ef626e42b2b6978900fed64cca013468e6c4581a230fe2b4caf52bd483fae320e04089236b913dcb671" },
                { "ff", "f667bdfe6b764433f5a8048f7796bd1b58915da0eb42032cc26a19e571602f86c6183d858a4809c97bea7a8f9d85eb37f9a8ff70c4eb680c08dd76738a991459" },
                { "fi", "53106d29643586eb2407e4adc36bffc8adfa58af6f67832a06bdd22f6fcbf3f8bf282c35e1508ca8e28c5d9d90d3be8cad5b226694eeeea96ac4f1aecd46458e" },
                { "fr", "733c964963adcc85107a87353038eb72eba0b097b9c4216c43639b370726362d3e6aa87270c0752fd503426fcbdb7a610e2025ad7e22c0ae991842a2ec8a9d3c" },
                { "fy-NL", "5b3bce901cac59f4b7a52076a953646832d9d493128f8801531dc203babcb5bf7c4d91560c60bcfaecde5b6ce7e758cc2752be0bfef6dffe4b06ccf24abf449f" },
                { "ga-IE", "51f14005c9bceb43e1a1c1358db72c54d101ec76576fc7cae5d2f3f7fa72f29f47cacf8fa65fbb7053fafe50b0e3bd378a8c35d61d2ef52a5125bd2f1d6bb35f" },
                { "gd", "8046966f4487d29ea342116d9aaebe7d319dcd4cb1195f0402c48eb99b7b0254fb1617be0580e5a4a623967975bbdb9369dfe2e8884b792d215beadf55776382" },
                { "gl", "426db42341c23a0f16ade54a7b556080c5abf8e042806f06bdbbe8ab7c20d042ec9fce67f0e4f9fe5b6d9b5e23382b29a07ec7ad867e3b488ad33a878f787581" },
                { "gn", "b849dfe0f47cb74bcb8a62e7c3e1b45c8b34b56ba222db76555c357c84ecdc97a262450da63575ec8d48d86c5ca3b250ec82f346f0af171f85f72baffc879133" },
                { "gu-IN", "116b31d1864fdde1f957859972d26eb8929e54add6121a263c3bc285b5e428b50033bd0ab4b40ff3dc63b149a5d629ca33da6eb47ec193766b7bd68e26618e56" },
                { "he", "1310b5098afb7da09ce48731f38d59d9d4a0a36ee7113e160d882e425d7e31b2235fc7455d3c7f553c53ef451c06689a0fc3fbb0dce8f18e9e93cafa4f8d943a" },
                { "hi-IN", "b9c565084410213a45a24fd8547ddcfcc725a92c500a4567e704015c8a8292f213a309542a268bcf2142aaffc7accbd01fa97dc5265cea3328a7d2a969d9bbc4" },
                { "hr", "00177e7fbff7d4d8833ae40fce21176ff964877601babb61572c0a50ec891314a9e1a799dd4b72dee68b480397a5b7f5aab25c56bb56f0a34ac536aa155605a6" },
                { "hsb", "e01aac1aeabc9c45b510e4595beb917079c2529395dfd27552bf08c541cf7ca2731ae4dae952be98b0bbc4469aba88c7d24d96b8017e99f5f0470b7a018a09fa" },
                { "hu", "34ee6a8f8038a607837c2f4e120d4176f9120421464b4f2668496ff4b7b50a4759868ea44b95e0567ff51b0de2f24b61468ca2566ad2134e264e08dd3a83ae1f" },
                { "hy-AM", "f3c6da50ebc66229c1248ad8749161b193b42eb3da663f9922f1260fdc3f5bc29750d5cc04cc1fb6878bd755d02b28dd40246b87f512a99dd9efa1b00958150e" },
                { "ia", "0a9c3daf12b0bb7137d8ec9ec9ac4c86705a8999a94afd8852e9993f0023242b6b11d64f28fc483906f1a1b3c6735da012c0918dcbcc20706904f290893e6fe0" },
                { "id", "132c9a5c2f087997c90ac8b26519bdd6a8fc67ffd3322265a2a9d2f7ff80ff96ab849fccf2d2bc1f56d23726d2309da6dee71700d18b9252fa2de84d3ecb2bd3" },
                { "is", "231fbecbda4f0527192c8e4be5dbe590d291f7b5ae857f6c4a57a8b4755a725af3ef2005c82f6e565b54164adff4cbad8ba886dd0d1e2c6581bbfff5e6685b6c" },
                { "it", "47afbf2ee897b987fa6a880c6de930582f11b32d5ba4493c5fa9f25bb81f4048f64760e31cbfeec6d36419dd150d8df3d8b4633d3d75d440e619894d72e8f13a" },
                { "ja", "90fb567d72e8db8b8c762721ecdc1699ea0937ebce28529124964de0547d8b4671a60dcb4437504f7bdca88ac2c389cf932feee0dec7328f12d8eba91b23ebd6" },
                { "ka", "eac005ac6f3148ac6e8616214d54d71ef5a4f37a8c99214f373c2b2ef1b67fe6a0e2aec27f38bbd426437f313acdd0e38ac1d5ba711093c13da32e0a021ba869" },
                { "kab", "ff7bf11322b7d1ceb54c06c1400ecd3df1628b1c60d7db4de371e8014952c465ab3fa00fcb719c1e8ab7aa4fa847f92b60df720aa7cd078cc8f8b0a4037f6099" },
                { "kk", "07b20ad6e67b9f54dfd90c9e8f5fe1dbf14f0a51894d54387e5f74af4925d1624e8be7c75c03fc5bef6c67606d4a1a65990dcda970a691fb30b2a91654875451" },
                { "km", "c1a66959d28552257bbe8d062802de278e185eb68459b90d2855eb49052910ede6f124ba9dbee14f2347a2a8dd94043e4821ff1cc65162ebbfe2e37eb9fb4a25" },
                { "kn", "bc3f5993ea172cbc157bc3bacc8b1192a3cd95f85707576a11d6ef683a94a088ba7f667f9ae8d51ff005c428acd1dd24b607fdad962f1794d5aa7f14aa30423b" },
                { "ko", "f4d7bdf4a762cc7fcc7cc02463a27bdcb64a93fc219f1fc10628208c4a53955042d2870b12a1accffd268ba3737ed4eaead2fd83fedd12e0a14372b312cf67c0" },
                { "lij", "f6d1cab1d3962e552a9be95cc6defe1a3be243aa6c6c512e60cce8e13a90cb5c4c53609b05592d086b82cd6e7d60b0c8b276450d8761a77f2e8e6813a3982bfa" },
                { "lt", "0411f902314661552e336b77bf4e7a5c1576108fd142692b6fa5847eb95e4e81a0e8995b2bd99fde28069294576173d6bd2f214999060371ff372307a2e52ed6" },
                { "lv", "af9899b3636966ed7cb7dac70952a93dc3750a962da7061d33c96e381fad7e05c20b2bcb4fcf0fba3a1ba0597fc9ee99918d7dbd8966dc1f8bced44e1d2723f6" },
                { "mk", "f0160b5d60923ee7f6031e85c6d5932b356bd276b626d762bf5cdcd34f91b4e59e0af808eeea5373dd95e426176ddea4c971e96fbe8832c54e2c2dd4e45916c8" },
                { "mr", "e97881a5ba1f14ed85e9c0b10eb440e2d8b630ac11a60ccbd13367178b13329f5c5057fe490d8ac25c6ec86a12676a29e13f831fe7224e762a571081ed3e5abb" },
                { "ms", "f30ffbfe9f0498960549faaffc18dfdd0952626a4ce5cb359b864f136bca1ec7fa74ceb98cd3d339aace2313efd69a35fbd128050060f526dd914d5f70cc9567" },
                { "my", "43ee00f40a1d8fc0f49464e2fddd1995e2de2383ae9508ff57f545b8d5703ccff7a63880f5c0b97dd9a701ab96182cf6557966e9758b7e967fe74206214c400e" },
                { "nb-NO", "1dbba1df33b7ccb29869724893547e614d7e271ae41a1bb43c64efd30753814a0be8cffe1d5d257b12b8c29598d038b3594b198e0d6d3c2df7d1c9cfb6a934eb" },
                { "ne-NP", "72de64da80a48aa5a686b5608ec56e417e998c74055e164e75cf638be2d6693dc13c35fe3374ac5c04b23083d7f1e8b265cc13fb3e578c6c4d581941eb4dd9a8" },
                { "nl", "66200c373ea7f2c7325c0007a2159bc7a165260cda5f5b53654e86cb4c8922c3b8eded8d383cba12d49d5727badeda342ea511340801822ef50125388c8a4a0e" },
                { "nn-NO", "ef6e373ff215909f2dc4b1092582f03e757c4df5a4a0fb7143897cdb17a3b4eacc12ae48528a7dcb6f4ffaacbb46a6db85b38c4429d3ba4a53a063c49d01b747" },
                { "oc", "49aec2ba038117e9bcc3a1d7ad9059c4c8a828a38894d3f69a03d7be89f1d3ba0ca113dca39121c4a72de8aae7ad37229e67a1a0396b24b7b5a089c2bfe53d43" },
                { "pa-IN", "185d1b6e2db401b3018df096c0f55ba3fb403ec2d5b0ff4bd00dadc4c5878f60256a600c229b11a983602d75bbbab676e116c7a9cd6c68b2e61941ceb88ff029" },
                { "pl", "056a2efebc0f3f4db1483b9c37dd283ed29ece7e82705e632b1fc5c8626f400ae78b0e3c9ff45257732042e380465abdfa4b09c08d0ccdba3ef3d67c89f3949d" },
                { "pt-BR", "09bfbe37c5a1a7e42d7805e6beff4fbab4227658360b79d7661ac09fe8babdba226c2cc146b6f670ac14931c78661c5f4ab96fa7aee9deaffcbf649a87b1efe9" },
                { "pt-PT", "0e68f01d476a8b90fc7d1fb596bc3592ce3d610aa0ec9be71dd82a806f9d8cd615557a2a6e04542be5201f3c0d6d63831e45cec1da107b73beabbc0a18329646" },
                { "rm", "a6d03b33ef4a1e27efcefb3043f16fb40b99347578acaad15fdaed66a16058dc583ae7b1a6c89f94e5d82f7147153d3d84a543beeb13736c940fcda1a75aac17" },
                { "ro", "44da77487d2d5068f84b26575abadfe3c58d056555d5516ad4ffb6a129ca8ca2b39f5484038cdea8e56d1fa8f1f1e0d7b76b2b5d0dccb2ef2859f48e846dcb4e" },
                { "ru", "a57aeaaa6616e66067f39ec7f4d22c07d2f9bf2a62a9b5cdb3991a4453c1e6686210fed6bc80acc79d4ff2d20f979fee61f8a35a9480d930beb165ce40330a2e" },
                { "sco", "2364f27f36cf99e35aabf47b539628047b5c0696e7622360c83472b60d0d15f951902fc54208ca02e439434802a4602bebc12fcf009d31b412de80a2988f5b01" },
                { "si", "34df68ae6c3541fa4d9786017d89d9c53317053b627a0dc8cfa560150a8b558acd61fbe52c42b3c38df31b4136e9d445fde739f445b7d8b15bdaea6316fbf923" },
                { "sk", "ad064e86a5e839c255ab51a21c07f9c6be9def2adef19c2852fdaedf47c7feb8842fe027e981ac50e31b7732337ef99904763e14f5dee7d73eea22cd23b0e72d" },
                { "sl", "4de0ed0ec5eaab853f127c98fb85f296c8335da3c92f85393eaf2e53a604cee18a50e76154b602d4a0eb0fdf653fbee5a15790ce1fb7fbf961a99480ff3ff01c" },
                { "son", "9884d4188287b51e62e4c6824d1c32d8abfd52c99e9ccb21b404af1932792196a3cc7cc00afd6f4c9e3e048dbc317bdd9bf047697ec8b1640386aaf2ec5c94bd" },
                { "sq", "fe0714ac1277980ce6f710a341c5ebe40ec217aa8b548890eac071fa84cf1700cd3e054af58f8a6bd0c4484732e0599a7fea985c4ab875fb536582698df1f0b6" },
                { "sr", "e859e29685ca340e816c53ffc95c795aa4a5609b2a157ad491bfbe088679632ebfb5588985ea51a44d6a112d6e66073a25f63ec796af75056d10b79a123158fd" },
                { "sv-SE", "996c755f923f815ced154f003b9d699d23ccff8873d55cd8df023e8f53e2b29e746405276f8a25f84bbc06e00db5f4333d6a655cf4078a524c24bf9ccc62d1f7" },
                { "szl", "33caa00220374cf1c761a1fa0b0b5b9279637ac34a01e617b96d87a6e35f394ffadb1c02dd3eeed3a92a80f6cc37da31df28f546eefc7bb6208248016d80d77e" },
                { "ta", "b22aa26650dde435c076f39a9df2fb73398a3b3a6a9ad6f71d90f10ae52eb46a08a59f1e2d540be1ef89c452b8cb1155695809b17945a7d7cca91140c10dedcc" },
                { "te", "7a85461f5ead06a9b9275239bf94605bb6f6da596d5f4005a890e27f04134fb47e0c7298cfc8362efa8835661a0c8b6f1531595f22b5de5889511fba506a5c0b" },
                { "th", "e0149f30f11eaeb2bb1583c83a0ace30044056406525db04706466bdd9b729e24bced6d27899930a37e2458b76ce1e8a6b9b0f7479702793d9b14c86a4916da0" },
                { "tl", "79c26e72efd89bee315ad492bbdf4285d958adb3f68b447e3454f4f9a56f32eb84dd76fb392f657a0e40cbd7b218aca7098acd0945c451ebf17a4dd745f66de0" },
                { "tr", "cb6870581013250d6ce5d6656468d86dcf747a7b869c52355e43fd0bd0e450e465e779ce1cfeb4910129db86210261ab5ba5b73f6fede3d643fe3100c4816ea8" },
                { "trs", "e6cccda210c6afbf43f639b860cd2be5d7306a70c4bd359c61c88fd5397d28096ca5ecb7b2688a211e442bc980cb42b026e7f7fe511cb1d7c131339a7ac8cb7b" },
                { "uk", "7ace482f7a01b8ba9aa8946c09c735e6fba67947844c1c733fb1e4ecb41861b8e6de863f79b0aaf4d3cc167c8395663395d8949d02884a996c91f1b6373aaaee" },
                { "ur", "3446749ba3c123c305a08ce1a5b92ca64415b627cbad98698981f3383602002f8b09be563808eb4f697b442c3eff6de9b18a4d3b852a2d97deec4ae371aec389" },
                { "uz", "21df7f32dce3ae7af986bc73a57a8d8797a073ac6c3fab945b0a834bef33a3a8f9575f9d8f44d81ac6d6c390d445e467fa1e7b6c6bb10f570da7b438dc9630a9" },
                { "vi", "439eb7daad002e762416272e0d89db225c48f38e6aa0fc43e0facf9ccdade49692cb2a8aca706928e93395557b020b6839f67d256503a0bdce86c79224436b94" },
                { "xh", "9c5ccd88a6f3230d5b111c5c8a2c03cc227538e7854a76140ba34c2cf63fb64b31b9d4fd3ac078498800dfe0b11a1f2ffe87a77527e7986b22f4753c080c0ce2" },
                { "zh-CN", "ce846732cb5416072dddfa2d323d4c331e3e8d73ed647dfb18a2905a34273d99a7de8b5a3ac066cd6dbb3518ed2ced0b808036682fd91d668a17d81d87d2c24f" },
                { "zh-TW", "56903ca7d678273bd92b494c69153c0059e451c473949c35d2daf8b5013810ad7b5e803ead9bb50c14e3437d7af13ce472d9cd9f33999263a6d8a875a37d2ac3" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/107.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "0b75aadc719ac1f185539eeffc153b8d015c93ab0caaa07f82bb241f3ccece2a13dfeb68a499679c6774ac4ad902dd8b2c21f12c706b13d85bb148787cc4340b" },
                { "af", "a8bbd65179e0ab2ab7bf2b258f5e8d1fc2def367aeee3fc44604b9ded052d12e5446bb22c4c5d9a5c78d47a5c39b9fff140481d09af3928940164651d63225f0" },
                { "an", "6fb10e68b25186324bdeb6edeaae5b2996c24f44e945aaa555c53e1141a5db2615387a89995ca4154638600ff1e9e356adf9942c89654d7fbb2a40e2bff6217e" },
                { "ar", "7242363fa5e6c05f096ecb779e7aae474d4c5cdc79251033a231c5174ee00135d841f6bc5abb40bed40c1516d0a43e3ddef0d1a207fbb4842e96179280a5c95c" },
                { "ast", "e6bdfcda6873d129e4b33cc048ecf00d3063696f306507d594cb768fa5d561403340a9e11dccc16d88d06db851e6d68a15d6bd939e341d992f6edfb5a8a35494" },
                { "az", "c4e95164bc7185098291993fe211165ebd0cb0ac56e9f031b4bb375fd79159a60e13fd0152d3ebb092448b1e14f19e9c9f2ca56c6d26d2debc5e2d50f20d9ce1" },
                { "be", "dc79fe13a8eeb0f5692374450fe62db03fe70a7f26a5b73b00baa4838b3da417f90795f2899bc68733e2e454beb54037aa3224e85cb9eae59272334e54194730" },
                { "bg", "f94672aeb5bcdd78639f96ccbbe203ee5e873f9cd275a1391d4bc98879e0a583ea6741908ba1acf326bf153e95b25236110aea4d8ff12b76a1cf4ddafd176614" },
                { "bn", "c24833326ca7eebf0c5818b56d160276e4d190d26b59113be505aed9f3b1057095abe9c1c752f4cabd314c26864137685f194913e92e147c7fd60216833c5d11" },
                { "br", "cb0ee6b2551877d6b581de10d113634baeacd3b4b0413aaa8c1fdcc193158653ddc7e648fdee576ec113cbe0f5949fa9731598ff88a97ac997997207882b57ac" },
                { "bs", "be52f3e71418515ce890e6123a0746dda56e1cefb4145f35e6ad5e81dfacfb37a459005cb2ed09b01361cd289655b44551fc9b997d23e7d225613aac20d99a97" },
                { "ca", "b1e45ce8b60c4b91440a6e83e1779eee6923cc23317c73bb38baddce89b1e18762d1c0e0050f4c002b06dea57cbbd0927ae31d1921930e5297e269d9514a83c9" },
                { "cak", "2b4c03fd300b37e3a8eb53453a8681b2f32cee7556f1f44e55593f7794b9b5ce4a78a4c41e419d852b40888870bd450219ebb272caf44722cc2ca145dfaf94f3" },
                { "cs", "0bc8ff69f5c3eeba899deb675ae1bc0b02ccdcf5408b6361500424a002904a0f7f038729518da115561c1e52a8ae8ae966f5e0a00e33389c3251a71ab7759468" },
                { "cy", "de48db0abaf49483ccb3430f74766c15e1978cf8154af0f811dd6915ed95159a6077de881ba8426f48f72dd606470afb0f4078cfc9d8bbb4e2fc0b422f023faa" },
                { "da", "69a9613c8dedf3d18009f24fa0fccc2bd746e21383514e14e0ce92bb09ea2fa65a3e81a324ff8f75f5277a4ee9de19cfcb90cb6bbcc24b81afe813941846772c" },
                { "de", "f04b1700a7bdbc8293cfbfb9ada3f430ed2ebf62194f904f3f8b00c7ccf94504bf5fcf5af3a2d1601aa92f8b04c800bee27032914b1a9b6c053846ac1c4aba46" },
                { "dsb", "c59a4c72e5e31975fd426d14540f1a4358a524487497e208848187357c249b55ebc3272ef2818e9a5844d04a845c3e6a7a6ddb26b86e878bdd00814a99ce08e1" },
                { "el", "b784d058f4fbaee31f721ba3b32db06b4128587eff552a3ffb61f7ce2e69f0b00febec7c9f61301847ed168db0866d43f8110d8e980766106417bbd8a7850679" },
                { "en-CA", "eb478cd3eda87b44e5759387ea8e8df5d2c8f9fb1cf16b23c989a98e553ec45e8e38685162e0759a35d1addb417c0077ef86a1c4aaa237c72e3ebfd809bcaa62" },
                { "en-GB", "8ccd03a92041091c96efb669051e6c6d7495e9827c890df457acbf18169a73ea67bac4356c8f18bfeef4539e6473595948a8d999520caee6fd3b01ca24d9b338" },
                { "en-US", "6548379d11edbce2652247c3150ee07ac7919d91397d50fed498fef02cddaaabfde6319bc2ca2b76b59833108f5b8bd865ec20a6c278da506d7e2de95ad67eca" },
                { "eo", "4d26f602e1326e6325ca0fbb0e176b40c4ae4304f47a1b81574ebaae989547bb08575fab69713528248237bf1ee13ca28b69b64181e1e8581cc391c5b6e9ed1b" },
                { "es-AR", "6988df6c93b1e742044388e903fc4a34238fdb3e4923f02ffb364dcbe5a47e30805817741082b093836cbc01ded369194ed76927226915d5e37aef6404783721" },
                { "es-CL", "f1ea8d743c484c4dd78d4539d4d5b9c7670bbfd685b9f17a1544df5d0433a66cde87ea98c1aad739a0b0119ed775c9219828c48b353167f4e4b2965f2be680d4" },
                { "es-ES", "70d3f43151fe5e7445c3aeed5b721d1842e3fd8e55b08301f1aa5ddc33cc04adf171dadaefb2a57aaa3c582f8c0aee2f804b53815b44d34d8c40d5462b515898" },
                { "es-MX", "db40f3be6b51f1cb93a4421ad46b734a6d1fe7d99edcca59730fa0bc200950ae522b67b9b5a4a4afc6358d488b64663b503d6723524385dbd6b6735bde15a88f" },
                { "et", "e34c5c0d1676b738e759d5a56ee515d690954e74a7e8b39f67d69d039aa7a6bfe0a94d6784bc6bc5da55ac481b774758391a3dc5009ac8bb9693924471ee7ed9" },
                { "eu", "e401fb91cb414b1eb0cc765a80ae8cbeb061052685f18b40bfb86970e947a9e10a24b81b93b970661cc32c7991092285b3ac5aa2c18001423957436b4f46030f" },
                { "fa", "bc0b6fd957b7d3259539dccdfaa5092632852154ae8469f6cb02d6ff9da7534892c4fb8f0b548770613e7b4414e73e3389d3603c180afff0fc15a4ed5becc7af" },
                { "ff", "62b2e2172dfbcccc811ad3eae1be9c0960c54fe2bf36296601285de48d468e630c797c6b58008bb7db4aba0a090e0151be26ddb533a080f2b2ddd1a727c2effa" },
                { "fi", "bf7e7b561a56c175adac06ee9fef9594dda9227cb2882b7e1886c5a5cd7524c491794aa64d0acc1e1ebe458bc447cd772dc0f40d2ecae5af613cded4540a3f81" },
                { "fr", "8f8e3041d29dd6506ead1b0f4068d0961225ffe42b91822456ccaca35b73ba606fa2e6c8cee1ce0ed19d7df63c5b3028cc613f3c69480bfac072957b0b346e13" },
                { "fy-NL", "0be52e94f31b243fc384057ba88c6b2fc60acc8bdde22d21ad95189ac294e7ca1a0c097f069bc2f4f2e97e32d975c7d9841e76ae6f6b79aba076281005bc8ef9" },
                { "ga-IE", "dbd28964cef6d2fb04c93bc39cf1ce3e0b7629c7f4446fc0926e6127b384e7b2b92e239549665f517f6fb54597a9fdc950db65c00b2a0944c48f35e11e7a402b" },
                { "gd", "0fb414231e583be8e863be93d90ea79263eb018d0c4e281f4a3acd21dad07c245ae4bdfaa0b73369191ed7eec2962574da353b59c92611d9b84140ab3882f7af" },
                { "gl", "74e8f59f07921db3143d29dfc9edc1a98f4292ab8e59d87bc4ee49e6ee82ed261e3129c7c8b4d1fe7b733099198c2f2dfdba77bf1aecabc3818be29c54b603e2" },
                { "gn", "7a86450a568f4edf1935ca941f4086135a31f1db286e6c406dea273c574da4add75aac8d1ae46a1c96b54a2ad2eea7870738d04db5b8734347a21baf0533083f" },
                { "gu-IN", "69ae93f441e4987b2e0443ccf54e29d165969b273d455de4124c618fa9e73bf39c7ccee65d8bea23ead3a3d555d6df9ba952c94eddd9ac970c3b0867ba14af21" },
                { "he", "63faa74f7584dadea24e1c8a25d1f5f28aad5dbaf868362681b7f88ff7acb2b691533ea5a4ba4b7403fccc8da66f2488157d6bb75cc983cde37d5a3e71ed9422" },
                { "hi-IN", "cf085276f5ca16a80cf93d5e22682cee977d17fe1f7789b19b7873e0b6a6bca4586e7511ef84bbc9cb823701cf3f8c65d86dab605072ecf97a8c2e386991dba6" },
                { "hr", "2b93f7d3353fc299a4acb628258b8da9b01292fae17f0035ec5cd2497e21f8675fdacb1f0105c7275d482973bf00f7893a58d09c4b99b042f8174dce6575e754" },
                { "hsb", "175a37d8a8a6123bffcbc587d116565b59cf9d7698aa3e14882a59038bb52518a0ed98dc0c934c789c598de38bc5a976632ce205c8436fe9c43e03ee55a94aa7" },
                { "hu", "bc5c53a451fb4ef22bcabc67caefb2ea88dca8af9c64213abef307abb347634697c7f79655a7ffc9590c465486dbbc38f63ee5fb777857279513e95f01a02cb4" },
                { "hy-AM", "60875aa353775860aa72d2b9221dc42c7ea6884572a5618341fbb0c1ee4ea072b179af6de012465892fc05c98747d96a5d492d6f815acb5fd4faab1636ba9746" },
                { "ia", "a6d7aa43957c48ee646fce24d3d0afd92615a3724afd0a6eb365364560d059bd550831f6cf67dc4e6a9b2b3547b08ea5d34ecf4551ff6341b3d9d5d12498c775" },
                { "id", "67be2af02054f306d54133c96dbb6b017dd496b19d7c62f0180a89be2e7bc6453fa6babbfb79d5b052199b4b9c518a94fde6c3fa7a7c413945f3105587351d4b" },
                { "is", "9e4f5510b6fc2fb26322c1d7ea793208cb279b91e8ff90317b214171bc3d4b122d4c904978d794dffd7a44424a19c52a012238105274a8537bbd6c3eced79ca9" },
                { "it", "3304152b4df6f85c0cf231382daeaa745b62ec39d82300cb096fa4bc748b5e3d328b79703e6acdd16742e538b6533740dcabbe832638e0ebdef3f5d7ddc3a695" },
                { "ja", "5809b2d305057478abecc6dc7e4f9f372fd9a120d5718862f80397fa4bde588f3a575ae4f5dcfb12278162de6c5e75b8f9c11e9e8cf4485ad4e6fc1726660d62" },
                { "ka", "0ead69ed74d47217755f47d41e686cac778f02083fdeb24f52b726efdbadc0b7bf45889cd62207c3aff1e1a8feb05f9154b03688b7e466cc99e57fd73d1a8d7b" },
                { "kab", "c6fb8df950ab0d46084f07e83311c7c5026d20298033807cc6d558d1b84c8df38f9f320e92324d32fe190466dcfbb3d4b001958c6f154a171b84dbc150ba6c9e" },
                { "kk", "f6185969784df158123f46a8e2afee5b25aed205422605cfc4180a2cdb8af13fc93941fc222b28cf87f99b0465ebb0c71536d0fc4579138b96d7722c20a7ad08" },
                { "km", "274fe439eaedb83d448e161b8b1521ad265664651d810c1eec1d7ca9d575e7e6b5e5d98c0cf42c94f22048d0c329c923f7788f620bdd255ec3b286515e433939" },
                { "kn", "de56ee0d6a31dc752b0a99a070cc7001ce4a8ec014076644c7c92eb8d57122c2c3b45a578e5fe527c5404c12d30300175ddb0eeb6d8783fd37f4f876763058b1" },
                { "ko", "a01c9b095e0e23f1b786d4926becf0116647d3f7362c0f6e13a3781aedab136c80d630d15dd5be8cc94025b147b17322972d491a3e339ee2ee3ff82c2198dee3" },
                { "lij", "e9f5af3b42ddd6cffed1cad2bce57c667ade5a93c57a13eceb6246c33d301e0e7c52483c57cf0ca2e472e0542566f15d755b1ebce14b2fee09069dc44346c9da" },
                { "lt", "efc4bbbc29263a38052990bfbfa5a8ddbafdcc94888bfb607d35079b655521370aa876b143fd134b58149af5621084d8b44e2a1aeded9f55f43eeafb76cc03e6" },
                { "lv", "5d3d82b439a08b2e3787c5150f6c310c907f6f43bed7b2f649bbe37bf026ef2a3c1dffa149d36e3cfdb44f29df78218a3bc433a995fce914f93eb6e8ce664a4f" },
                { "mk", "7822b7edb337fca1118da799b26b73f66d01e5fe517c6831b3f3d7b706c3ba16bd91aa568a5e6b3df2803957bcde90f9b524adcbe1bd4f0be71d69de7ab7f44d" },
                { "mr", "4388c883e349b9b383792e8d300de5abaed31b5c33b96c099264af0986fc0adcb2c4d8b218de421bdb698f4e5b87b8a7be1e12833ccf4e57474fe3980f3d2ce0" },
                { "ms", "6b743cb52a1214bedaf048133673bea5b6ea28d422d752675afddaeed20c8a4dde59b08868957fc55bbaafd98bcd0fbddb61ac53b5122927669103fc9f061645" },
                { "my", "a7e6099cd1086b475db21b5a0b77792cdc16434d52893b112df687fb0d003dd3af626a7ec9f7fda5d76798cd75040d48237d2a4c6f18a5ea59579175ca5f611c" },
                { "nb-NO", "d77b4070f6fd0bcb27aeee4e8702689cd6c88a676cbd9c77321a516bba82aef212c3505f6c88d2f04459b64f85dabfef2e4fe2fcad2d17cf8c3d0da9d4aab5cf" },
                { "ne-NP", "8fd27aa7dec2ae7c2a0164f5d4b2e5cd4672cc8a8aba30bbe3d344b8f2a4de9c7dbe4ba9e87fd50bc5e78bc59f6c0c7b16b3c3bba848194e6f7574ae00634f89" },
                { "nl", "baa1f89720eeeec331ac48977d59c99f48f34a0cbfbf1e47f1b46a003fc44c15f298c98736be9823bced67fdc580eca14e1d47a449dbf15964b4601ab48bd517" },
                { "nn-NO", "ad3f6009683297cc7924e8e650117dbad86a59ddd76cdd55268be4c5c8a6d63d3a4ac23688a33685314a8feed0634af52f1ca01896943613dec76664ead1c26d" },
                { "oc", "030736fa6954ce4e420bff532983de590530050c9fc9dac086f445b45bc25ad771bcf367354547556a500d798fbb06fd723980b41647fa2b86915e0f3adbda9c" },
                { "pa-IN", "8066aff373c3f700594db5db479cfeeacdee04c29205e50171070d63adfc57639bf9787663d6299b6134ba3e472b940f0ca3472727cb50ca661aa5515038d7a9" },
                { "pl", "67cd83bdfa9c9bf993a04e20ca96f6b626d00b3d470e815a18371b4ce2893ecbd90e15b83590c35902845e44e57e2d874863044c06a251f6043cf338c1320bc5" },
                { "pt-BR", "ef44990f986623958cc8710d4044b88d4b73c3a7ce1d86877313a646d82eafe18059f31767c27e4378c7e65d376dac9ea86624f979ac538fa2d03ebf526f0830" },
                { "pt-PT", "11f8fcba6e2d816dec9095f350e38825da03925d1f0f0e7c4356e082ded5d8e240ed1a250c96a8cd1e33990d99eda937aff4f8bf1a65342074c0b5ff4945128d" },
                { "rm", "2d7ef3e4f6a01bd959a461ead8d34447d5b568a28f01e85a598e929a59ac8c71717a7f5d9936c6005de653fc2df9e07dda7a198428a9e2bf41a2e9dc9d4d24ef" },
                { "ro", "e5f15477c8ac0897dac94215e2080100e2d4692dbf3d6baba9907c06b8347d1e8aa46996dd4b5e191d16a0178710d50b52cc38f106a6df191d78c3e6838ab1fd" },
                { "ru", "c35dac69bbeb129a2bd68daf2c811c74f37905eab5072cab5db63484950aaa7cb83468d65fb6e8e9d3f76ed14c3b130d4e72bb71580a6374b0e364f4b62b605c" },
                { "sco", "38b67e7f5176e35d1f9b93b8b54e72f8b7435b36220da1abb579ea2191832dfa6b203731fa658ce2a8bb95041363185e6a988eff577a8ed0404c260d9db9a474" },
                { "si", "d2798e22980dd350e0ba6c308fb7fbe0897a1cc97278452b47d252bb701d63ccc1d3cc2dea6d67e57674273f2b31460028ebf87a4888acd8616811ee072ea252" },
                { "sk", "96470295799fbdcbc2eeefb1f6f3c200d550c0f8df3ce8fe233db237228ecee60cc496471d939d7070964bf8d660c87838f90d929ab579fd278473410311afef" },
                { "sl", "021c0c773bd2eb47c50bb38559c28b91328200a02d284c531c556efb51741dd960410db89f5d54016c01763403865eee6fc65e539290dcb5ee7bd19e618df059" },
                { "son", "5a4c7e9d699a38739aef86192d7dc896ee7fd093300ddb30b9263f921605a83c7fe30a107e32a6d5b4cd399082c6e8dee248087b2521e7ff197f909827371da1" },
                { "sq", "c42173c11906119c7824910b0ddcff8a483a05fa45bfe35d324fc8d8076f96b1e7a5e51f5c3a07dba76825f2ffbc08ba3262299c639d47c09aa721e82b90e58e" },
                { "sr", "acc9849ba20e58c5721062dd994243687d4c148c421be241417809fda3c852ebe52abfa26136e04deeb9efbc42d992fd3f471839445bc79ad7fff48abcfdc5fe" },
                { "sv-SE", "4f61054fd6044a836b45e16c13f24618b7117bd9729862c4f2b70e5d581dfd3bc06ff3394a482e7555260fc594b1529791faad88424ddea7c9540091f01348e3" },
                { "szl", "fe879c75a71d1a44c507f1398560e34dfbc873352dc95e0c8f15d9491d938ca36cc0273f5fdcba87b85008dcc291d20f526dfc45569962689e51394bf2646dff" },
                { "ta", "fe0cbae99ada55ec43ed16ff8df4126f737b4ccff84d1a9192182454bcc04eebe29574f6d19d3e9f557c07a99c1e7eaabba12ad5a3f48322976735efa8eb1b48" },
                { "te", "4fae83cc2dad944d4a252a32d1e777f0a0f335bc76cf64e10b5afac27310274362bc94f50e5ebfda25e1241953eabaf45557172babf2de87f87a47107d735b96" },
                { "th", "9072b8f7db5aa93293745a96091b344243a2917c17deb6e128c3afaab725fda1b9e11ceedae571860ade7fac940fed7d6de2e60a6e51964b827fd11983848664" },
                { "tl", "d9dcb7da94f4ae990b9a7bdcbcd18c9a04a4065bc94ceb7e9c0af00c01f8a22156cd91459ccac7010a232ee04ae00d4e43fdc77cdfc24cdb07eafb41b4e8813f" },
                { "tr", "d81485ef5277aa2e366a0f498411ae16d26a64b4c560a0c52d01c73508002e789ef1211399f605d6422f69bb6b974917ac873a1f2268c451dfde08cbea176dfc" },
                { "trs", "bddda6c9bda7396b562162151081a2989d39671f750a134f3aa55de780d3784edaf63f342297a769cb7f6a572c0d0017a742839fab4b5b1e2391afd33122116e" },
                { "uk", "df68c0194e8a484e5d907f5dd25c0590dcf5d3413619fe0e519c54012b8d4b6bab01c6565e76f2d037499e4b919e3a33f282f5dd23ffcd824f5db905a0967160" },
                { "ur", "db9e51aa0e0ca133f66da3785b9755e2791859226fb812716450a9ced95d44f5fc833e14081ef63a19b758fe15dc0d5a702f1f8a1210d6354b936dd0845424eb" },
                { "uz", "432a238e61646dea660b4281a01171ca58315760c93469f6caa8a67a7ff6dd510793d11aa5624abda10fdc659242db8cc5f3eac17ea08ad284ece206fe731133" },
                { "vi", "b532b2ce76858da19ef46c989431b9d15130b877de007f08487335946e941d7de384c651cf981ca81a5853c76455411f1a968a6d9b1fc9da9f2ea31d3dfe667a" },
                { "xh", "065322a1589dceb9501121696b4e2ff5e2440b46484046bd706c77a8931283a0a0e4567c9efc3b4967b0a65a33bd92baa1ba88de09878d88d499cc3828901265" },
                { "zh-CN", "6d688e9c2bf17b6fcc2e9b536fab63f04a2de50f56d54c5895cf5049e1f07e6c40ce28724e3b9f8c334218fa550424d0e635aea3d73044cdae9d5233a746a1eb" },
                { "zh-TW", "50bb3722930d9a4c7bcec68958dd623a185c73be85b0b985143e92e35bd1451cec747951321787ae336761495517ca14fe22b1fa54d2778072334fdb0c46d8ca" }
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
                sums.Add(matchChecksum.Value.Substring(0, 128));
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
