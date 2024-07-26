﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.0.3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "919ae5b51f5013fb0bfa601b20572569733345edb3a2a64f004e0d8cfb428287d81377ecaf366c8e3b07dd270bfc7d58e38fdf9de7b4437da919cd7459fe4bab" },
                { "af", "eb849c9ff395d564afd92c496ee37827ea997a7e3cf44158bf73c4007c33f745155ba37ec120bd4bd27c8862c3ee32287dbfd516d4eed2573e721bdc84d87cdb" },
                { "an", "613dfc2a38c0b188d7fb29de028634e0331dfb58e93347bfbdf225be70ba6d18c37188c4bc6852dfd7c9bcddde80175236d24595d9818cd701fc91917194e530" },
                { "ar", "66ef745d080cbd0e203e50348bf892ee12f704cb963aba974ef3237bdbad816981f9c418b04455508b2b485cc6f52c243e69f40cd8eed72e87406dec251f944a" },
                { "ast", "0470093e083ee54ff8a1c5c37cf8283d1a954deb0774916f2765739004d50840e57c6f1768f94fe861d455dc1e871e73b82fb6d385e0c0f9e073d3aa5446a568" },
                { "az", "e03d8d785176fd2cdc716ea45177aa53e7bcea80f5e41a59ebfde470e6f511866288b2cebc40740645041c14063ca5b1b478d3a1d55b8d8855981c01e94bda45" },
                { "be", "acb2fe628ba45c12165118353c0d0d1963d0b516c9d717f9f99e193b798041adcf3a8d0a0554f74cd3543f28450c667f5d82baf0f1461e209886262d118d185e" },
                { "bg", "125bbf8e95a9b7bc1a8ce0477a13a0fb0e755ee4bde7fcc71bebd1aafd735a52a905710b6c5e3d7f26ae3608298e2066f64d64a78f0a9368aa415b97f129b4e9" },
                { "bn", "3c22990bfc8dc032ceec7667d930dfe41fd180bcfad44c728bbd2bbbfcb47835e0f5278f327ee161fc932a4971431e2849919ad9b8f04c6a6bd161a26831ada7" },
                { "br", "ca93978cd7bb87b488158f1f7cf132243114b71acb6ca981cb2935d4c9bc2be6514ef9a31b818bd717272eb3aef165cd07dab0073b48300f326f6fbe54f271c4" },
                { "bs", "b9b1cc8746b7e0c76cb1fa6491743386e4dd3154b367a8d572fc26d7ba9b2906b826d4e3caad3467a10881fc50e46ae26df3205477864b697658c80a4e87b5dc" },
                { "ca", "09db6b402b3ba4a870a05c732e1fe497672e4c5b70f0db573f7c73f4acc188ecd2c527e762189f574ad6a3200027615c4524e9a4b7c8e3eefb7c6388a4f6ff03" },
                { "cak", "8048c42693c10018d8862474e6adc9a4d80285fc2755b2d8c3459a6cebbfd0c7bdffc3f98024304b9708a65041045cf8db301de4a942ba6d57419e7ed0de03ce" },
                { "cs", "1be5aacfbe48efe2a21f8772de63ee70213382f4ab18f0e8df6d79b63548192c42bc5af83338e86273fbc235eea78f0f3e355ca2bde12deaf9824268bcf962d3" },
                { "cy", "3d4fa74fb737e369544b3a9ff0e077a14218772c699ee0a999c171d4a51f69274f8bb29b0327083af22818e66b50603ad4ace5573391c642ea82b01135eb9d66" },
                { "da", "d1221522055a659b331ac8350d969f20617f1ea4afc4e6bdf57ebb4e6b5376376720a4e52f710ef2c2b18bf760581d7e02a11e377b19d55b8ff3bc504a4ecda8" },
                { "de", "9e76a9dd8cc980974dced2d5241b8d5d97f4e4a5650677743c2ec73f154ac072116d2bef21c46f7a6b95a3f63ca2051d951d0209d5191c2270cb5cffc90632bd" },
                { "dsb", "3e883fb6b12bb54dfe526d8a5626fc4f56fc87887f12106408839a1fba2b541b7f66d3a6be626471f959da3a6380c83c1c2656ea3a7cf51e323cf543c4769987" },
                { "el", "52905c1d6dcca876821f8a193297d2e2d4d92277d7354bf8b9cfded91425b9eb03bc9b1fe0271486c26feb2059a4a12565f67aa4cf5b9356af5042e1a4074dac" },
                { "en-CA", "f67771f869ee536436a0cd5d73e9954b4ffb27e560e9865499ec4f028c754fdc850ac585764b47ee041ea8cf739f28d6fd36c28292cc09609ad9a87b964b6f05" },
                { "en-GB", "0793d1756b8ee2120da454c9c4b954d566df351e8819f7b50a717ae4cc8e33b688e048ff6f076d680ec358c2baabe92031838660c6183d87808ba3c1dbe9c6a4" },
                { "en-US", "ba9385fd6142302c6fb6df4a2a8e2441538da0ce49d126cd853be1645a62dbbd1df2d8799e149af9889f19484091f135f1d01afb20120aedaacec4e6d2f6aacd" },
                { "eo", "18049fc8a93955300e9991df26aa65793c904e3bbff04fe8649b4a7ba3c4ebf9223df80dff65ab58b9053c9d6ae4e0db8ae3b6626e013e86ce862fdb70f5ce5d" },
                { "es-AR", "c0e1b05415fcb56124aaa9c81f0078609a77a5c38321b249da451c2a194937d96ddf1b15ab46a443aa7d00096520181482bfd2cc443699bc457074acb90150cd" },
                { "es-CL", "da24075080c36011f5a84fe9626a28bd9377e93dcab7acf0ed92d6348583482e1da257c781154e53876eff0e4b983749e0a627a8f31100cc112412931e8366e8" },
                { "es-ES", "ab36f075f05b70a9e3c6ff1ff82b7e8cbad585b57e6d757560c841bcbe5a706157811dcaed576c75ae011f2e8de9f4734338ae1b5c4c91dda1d6d45d2e108317" },
                { "es-MX", "66ceb2484ec70886fb3f4348fc70ced6267cccf2c1680e7eeed489aa3e560c1f779e1ce9fdd36d65c1de2da0ca1c850e7a882fc973b68d25f9931b283a8bd071" },
                { "et", "25a0acf17a8d82e6807e34b0aecd650c3364fbb0a1c7da0274918df5761fae1355874cc4235378cb88612be8c670fade45a4e502f8095d980c3500aa2d0355da" },
                { "eu", "e8b5dc8d369708ba0b1964b6e2e368d472b8bcc279920550055e8e732a8b6a8bb972430271cfc8be024b7179a21bc0ac4af5f97d14616f4599c6fb7c6faf4173" },
                { "fa", "43e0ca8a34a84a247eaaba5b8dcf11b90c754f1119e6511c5a688aa45008540c504329254bbfa5c7dcde67e4cb8c02197ece6a84aba4265c02b754c9fa38e673" },
                { "ff", "0f8197b8fb169a4309f88c717d2e0e8afaf765d85c18e71fdeb5b426801041a3a91ae13d57a5e05a7265a880ba6156e74d86371e10312bacb9039a06dfa87e64" },
                { "fi", "f4f4cd42cbc3b163aa815858e78e75870426656ad38874c78162156482ab84333b34cc9db1b5051d0768287261c9119ecf695fd919d34b0a8ab9bf246775c04f" },
                { "fr", "e5642ffb930e07eacedf04a011451d83659c69ae7d8a3afc9ae76e48b4ad4439dff0a9bc8d4899b06fde5e7749192abf2b6d5e783d907a610b16aab62cdfdb16" },
                { "fur", "dfeda352ae4e3405098c4fb23c79bffacfc8459ff5e512ce2cd2513a93adf98c37bc3b36c2891161c0649c4e4549ecd163c617fb5411ef22ab5d4bb0e8f9e3ee" },
                { "fy-NL", "9cc00e54013b5e9acfecbb63344b507071c84ed05499bbf8c81a243578b95e981c000050acfe1151258848573ac2c6ef02d2bbed994587fc7e42a3a603a22758" },
                { "ga-IE", "2e0f575c04e3123427c3dfac855c376fe1d3f293e6c2b885821f9da83addffd2d246214472e53292a14e076c108d291c6c52248ce191d8f0ac583259c98eddd4" },
                { "gd", "a412999d6856616955a71ff87dc0efff33e310dac4a8bee086e9a90fb6de908fd747460e8b1b32d0c116234e48868d330f7df546acb8b9cb97a9e3d4ba2472f6" },
                { "gl", "368cb3cbcbf4a2aa3313f2d2654b0445cc79c9589312918405c398b255fef079dfaf5d1606ef4c7fa040195fffa2810e08e4242ce0ccc037c0cf0a9fbd579817" },
                { "gn", "27d8a8663bf830886b1d1cc1cfa4cc38da4bad293ee71df668f638a9f073557587155d3fca55d064f44bff260218dd240168e31e2691c216fc45776dd84b07ec" },
                { "gu-IN", "bc77c022cec0b065d09419fa456ddd84a923189d13d0935f72fe96e10914af9c31700c8ecc9833f60b28b6b6d31a858d947258f0949cfbdc24e36be5590d01e2" },
                { "he", "b72a7afd3d8e92a0137965e71c7c6548c6e9d8e29aae7b91d19812093399b3239291bb15e9127c73c219557dfede28437b327ec2504b11870e73cc8012862ac6" },
                { "hi-IN", "6705e177a66c1547d5341d606898d3307f23b5cb3e5e02d00dc8364a8b662da7d618d44d993754a7cdaed47f211b98faf193d477026e8263b18fc2c96bf7f9fe" },
                { "hr", "5f35bfe95f5a4c47b07c5ee9bed96b8724f9d76b75a5111806cb52e8c986d1bd3286325f36a037c06760e9c8172f0ae11a60b716ebc02fb063c1c490207ebc72" },
                { "hsb", "2a5197e0ff1ecd5fb61404c33bcdf5b21fc985668e2928acd350678651bed6a97b52d0de27fad3d95759af71746364416a5dd59eb8dc62d9f7b94cea1bb7f382" },
                { "hu", "cfcb4ec4e286aa2a7a984d9c6aa8e8534731667031f388861fdd39e0ffa3a54e072b27c3d87113e38a7d7c280504ec0d69ccd272b4d29676682cb3eebb624d68" },
                { "hy-AM", "18289b045c7910e81323b64fffa9c1ec9903531fef0f8322ba83fcda40f68cc805fdc810935dda1aef501cd68ab2d4ff47ad93b7fb0eb621d1ee2b9232d364cb" },
                { "ia", "4b71986a9b9e786f398dd616fcf3fb31536244fd539fb07ec7eb448948fb3f041f1fca3b599dbc9ccd33f967cae306677935d7ef8d112dafe9f73dce2ab2b388" },
                { "id", "ffb89839ad3fb6ef0245d05f267f56638bd1a1591ad4731ec6b6169eadb4d5c348cf4f978f3b4c0eb2d1fae11260a2845ba25919bc437630c40a8e632fa69f1a" },
                { "is", "225304283b1e07cf42e3967c6d05359c03de37b40b54e8d79cfabf676a77d4187327f38214ddd04a975da6856a99f1c146f73c33b9b27b8fb8da981bd5f0c13e" },
                { "it", "124f74e6785d35a157bb9285b7e9b4ed64c8943c02b714e2db56363d0cd990d84fba6bde3b3e473d25a5cf557483b868d374309d6ec3b39711ead85627357282" },
                { "ja", "d05991736f6221e3ab555a8443fe1e04353bfff77ebdca8646f224182626a7601e41a244a6de12160f6342ec8ea20847d13b54a2b2ddd0c28cde172ea76d3468" },
                { "ka", "365cc1536bb199cc0c6739b311749b9d8e24f6205412a06b428f85626e69b8a3630fe7ed573e51d4fee5095e3da08d61b9b99f5ca53f8ac9246351959d05dc4c" },
                { "kab", "429a34626ca1be96c051e57ebf9d5eddad034c04cfed57bf6f5541e437cbf89e9dd167f4dd0c4cec818e887b537fdfa4759a675629dd61346d6b69233aa26f5d" },
                { "kk", "a88d6910f9f083f52eba3445c3722989da00aac914e34ab7222442cff938484324b6b7c97b587f00abe2cf40278145ae3d480687cd24afdb4a6e560a58e938d5" },
                { "km", "4bc3f1f6d593af6e0513381f7e99c47d4439b2754f0c6489274f3b7ad1aedb38521e344b6ccd14edb2f75da678e360827c3e6e1b50c42138457a3d662b5601da" },
                { "kn", "3963959a265a0ae34be263be844d1fb72fbf1a23874f4bb489cb13d16ae43e77c3b55dad8686a1bba6000975544feec7fd43c9f8d8e4bfa7fdc422808203ce32" },
                { "ko", "914a474df1d49fb7617c9985a22409b3b4afe05fb5277ae6beffa89b321a789c077ce1e5d6774a970778a43bed329352a01bb71d207e47a98e967d23080d21b0" },
                { "lij", "33b9421e67d0f5e1cc35dadf1cbfc4a88fbc8a1d2c124de4b777023d9754ed513aeaade214b17dfff5041c242386078cae6c18fdef7ecac60e1d402a217a72c0" },
                { "lt", "19919b405586d64c1bca63ef50f4645b44eb9bbc67adf29264d1b30be0c68ea4d3726f2d741a8e22665c21f1c3950038c9a7f2b62a9562052a1fe5d8fc1ba37d" },
                { "lv", "41ea6407998aad0d60bde66d5ef48aa16ddc7cdc5cada37e6842e846f83891538191528b52081b79777c6c4f558195128e980909e3aecdecc591120d24e61841" },
                { "mk", "cfb9de8c03162a85d14a69fe60556cac5eea9bed01e439adc9b4e2297d75206db9f4893bd0a741542186aad0399e0f0b8c6e6dd59e3b90d4f8ff87c3c6206845" },
                { "mr", "1449feb67d1b47ab92883083eda8618d4b04e91f57976745448bedcb19df5a1ed344fada4f686323820700f86aef4a49eb090f36b9b0567a164142a79c34f0ea" },
                { "ms", "9f1fddfec6034360a8495fb9895abe406e906d596af31114e4cbe1ed5db74ce6c8c30735ba1e7ff10b1befa0b2e5cd4aa2b88f7fe4b64b1e1ecc9b2b7d52228c" },
                { "my", "94766bcc3e9a52d693c49a7085ef675e29ec73ea33df845ed6a67cbfe33f16cc659d5e25cb49aab69c26bb50a761f39f81d1b44979e5adcfff9860173a7de04b" },
                { "nb-NO", "26935e5083e4240d1a12561b650caeee56800dc3ba0b15c257e3e90d964529f351542f00b2f83edd78028a1fbb744f1994994fdc1c1535bebcae382ce0ffcc55" },
                { "ne-NP", "67c4010b576d6f4792f90d2f14f64ba16db21ab7ebc42f2419e3437304b2c772123b69997b9f0462a1013cfa34edd18370fdefd9058446db22ff7a1bf5413598" },
                { "nl", "de2a8fa6aa357c3a81a887fd574bcbf0a7043781cc7c17b165a56b28bb99b6201871eb1f7ed66db90315549f8c942b4361eb00da056223177a1209bb7a285487" },
                { "nn-NO", "982d8773104595befde489220a5631ce046a472aecff15495c27989c3d6d74fe9805fc5a721d27a12d72472cc1d6bb9576e304340f3975c1dc2334f82cbfff7f" },
                { "oc", "a967de1f890d9fba64daa54b54df68659bb4f96d76b8769225f9f70ae38ea93f722133dfa37a3f53bb4b651292aa4578a73751efdfad2c68165ee3bdb78f281e" },
                { "pa-IN", "69021675cf70affdf7c10cf6aff4d6f27ade7218760c1e824fb77688ffff453a32e1794f589fbb06cc6a684cab3f560dd59c578536e328c2ff3fac3737d473e3" },
                { "pl", "9bcf5bbf7da5e8d4f1895bd0586d6b5c80bbbafaf415c8ac3a50cfed080aa8e20b905383aecd308deced76d7fdaaabd9b25b82f548e2565dfe51f6acd2403e75" },
                { "pt-BR", "aecaeb8b7dd9e479b573271603ccec92740aa0d6d49ab049d199e4bd4c5564b5a862a5194887f1e29d67e3b992007189828ac6122fbe60f4884375d0e497f92c" },
                { "pt-PT", "04e251bdfa16c582d2beab6d68cf0c0e70ec79aae9e49fa80741f658ef9be2e1a3f27259d456257001316f7c970fc272b656b57617c17c92bde4efabdfe991dc" },
                { "rm", "faa9274feb9cd9efc3a66ff083f6e4faae9952cfe16d4f960947534ef91b93d76602bb34a2cfbd0607609b2d3e80f74bb274077184b27c9c0af200177144be01" },
                { "ro", "b6b8147bf8f088b0058a3fb2b477590223e904b67771a5969decfe5ee556181d998bc89515d326c5d527c02e990b39ca9356a2b9047e29e04dafb989b30d8e52" },
                { "ru", "72da1fb4855180ce2d04e5b161c4e08ff6620c4a5728cae27c7277fc1bb58059afc233276acce5276cf1a0b447b16e7c877508401e9833750d72200d4afed09a" },
                { "sat", "22ca35e9c5181656e5f969faa92f4ef1b7c32711178a96e8160480a09e76162a32fd1752838e8c9e009b7ecd5f08998150fdadd1eb80a452c553f7f34fd89af6" },
                { "sc", "48c7fe73e11dcb835c17fc8ed111f224143ff3274fb1412abef153ea74ca1bcaf7238d7d19e1d037c760de897ede52d8830ff99cb0403c6efa9bce23525ecd52" },
                { "sco", "25a856e845282ea44ba8864fb9e418bc8477c2af48968c32d9ac850410218ab93709f5f5dc49f1ad44ec3ca98c0498f08b28abd8ec3196ff5ce57164eff21e7b" },
                { "si", "1808704fbfaec78a40a0227cc079c3cfbfc21144566249df466ff37e7a44ad04dd7c70057bdd6401f33bb9fb58cd63ae59776de43a253407af13608ae6feba03" },
                { "sk", "bec0986ccc1dcfb6bfd086d113abeb6429f2efc3f3a6551737225b24339f44d799d79a245643743198387fa851b906e81bf2575864f85f0e3a5cb62b3d70dca3" },
                { "skr", "dbe6ac6aef5a0d566bd7b241c0ddd8386456d9fa0913df30965d1b1f3d00065e33370fa7c7aa8606351099611b01e87a9fe113ffe75f8c1fbc075f434792c26f" },
                { "sl", "f0eae0af3306c3ef0ae4b2a59230dcbc7dbf1a2f001747f6a2576bfa9b2187c72d8a9e1e34c2e303397998ce568f70dbbab5e670653247e7d67d7780b19ec911" },
                { "son", "71e316f4d382336c250849d5748823a33858aad3e1feba9dcb2dfe947603881273e95d6ce367ce58d403b160296bbd26d330e64698e9897ad6fee8306be68690" },
                { "sq", "61998a66278dd550485b99c401e31f22ee1bcbe012f67eaee724ab89c500d87db8305091a07a0cd20faf5550979a6718e3596a1e7ac9a4502922b81461b9694d" },
                { "sr", "6181c60313f335e593477b47a02b3c274a13a64302cd28a994b3f606ca5475ec89b7debcb9c6ec7d29deca1d19643f3f478626c099c3178fb7d87975538f9d1c" },
                { "sv-SE", "e9efd3d9199a91d8f917828e97bdb86495ead23bf399669d437f0da0b2f26fdd40aece43175f24cc344676906e14f75e52339eb50634d1929a9e0c63f77c242e" },
                { "szl", "5d92b7e224e2fe49fde9baa599cee507900e324cb49b2cac0987df42d9abea401575d21b1ceb1657da9aea5f797f282bf940377eb9b5e2d5ec244b771429956c" },
                { "ta", "f970dc47750f3d24874ac17741aca64ef0df21ae139c5335561bd2174206704fc42fde401a5519c2813c1cd5dd82432a14e0bece3e9c7b90107f5dee47a30a86" },
                { "te", "2e045c4ed90aab2b9a31ec8d93ba65f742e3f03ec449a32227acccb4edf5890c7a706c286fe125f32553462ce072df8ca45082d3bc4a907dd6b9df23eb36ef57" },
                { "tg", "c9e6cb4fc825c76fa32e45a48e702736fd57a451b5b57e7991e83208931b996423c8e0fa5fc7795a2daa479390e3c89eea750af186836ecebe79fb670274fa44" },
                { "th", "82db5eb1d74523c345ee21bc37828867b0017e767225b410fb487d8e3ee566cd334d6f2d4cf02d6c2bd4f2bd4b24761c73bd6b40910f027b50be1f2bf282db60" },
                { "tl", "49d47ae4248a23df1c684be095672d0c2d8e9ec6c6127add13d8e9c6dc24c411d47eee3ad3ee8a20d081472aa17f2b7449e9a6538136dc1177c5247f5c659617" },
                { "tr", "44882969637f8f3d8dd969a77771743c8f3817db5b4188391c40afeae32d06ac733ec8167f0770bf7e0dbb3e8532f9c5f6c2da10401b5234d7b486f992678472" },
                { "trs", "13cfb38446b976d4d2c4ccfa990a60865156011f3aaace101ffa745a2d06cce3da11ffc594c62bb3c531034810586e4a6f6f7b2e88b13bb6e0643f4eb0f3ccd7" },
                { "uk", "2cca14ea868408eb7db424eb6a409636b69e0a8244275515641a06535425d39a5b0d1ece2c8f35b0a2a95b8e9c51c07d37e537902a3304f402f4a6fc0c8fc7cd" },
                { "ur", "428a5af456517ef0712e704ed5302ec985b2f5768bd1a325a07a64fbbf9999a91262a4844890790fbf33e39a03fc2811fe2ece883bdc8134f258f8afaf36435b" },
                { "uz", "f4a275de2cf78247a8242a7a589ef3baf7286adfd7ffff87741f3b52e0f2f71115a1d4a0e95344c930c958c83cead9d006f282addf1b68b1ccfd61298f23032f" },
                { "vi", "d6ff6edaeebf98938512ee7bd0c33763ceb21b8539b1fb57021c258cdd397c7a9aaaffc3e5f0c46d6bfa50f61caab8fa93cd3f16e86e71a167a41e07ffea55d6" },
                { "xh", "f2bd2eedd2f3dfeb6261db17779ace64b0908cdeedae61225de2c87944513b2a9babdc8d6ed3b4b12b9691c64fd7ca14e00457848fabf71471746031aa2d22b6" },
                { "zh-CN", "edcb2807e7acd64d903e014047c01237e65fccf52864aa394d270d18560b1ef1b53e7e5d82ba1f0ed0c891d9938b491e0e38f42cbb9334187aae26e9bebde273" },
                { "zh-TW", "1792672c4d07f3c05acb4dbe508f6d4f26833099f8fac0c2841561fa6f745605db8df0cdd713981b26cfee6cc38ffb86f3e91fa610558a8918d9d7bf20f9c1ca" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.0.3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "0da535efbd2a176d85dd4adb4e408ab398877842c795154eae9d4ae11836dd95605b80481fa829e3a09dcaa0f2a03c4a025d2365ceaf3108f3de1295f09b9efa" },
                { "af", "8d82721110bb408ce7edfe33046903f8a704dfeb33e8e565394a86e7d33fe8f56eb9d9549852bd5b349c45231dca12547ff484a7d95fe0b91bf0eab1fed6e631" },
                { "an", "2839a93eb2ad6cfce43000f2c9be463fe66e882ab688592c2b377d82403b402a0e068bc26fcbda2d0a5fc1c10e049b0805640b5172b9b083ee1aeeccba1f9aa7" },
                { "ar", "cc14ae3c4f1c72d5bc1d89963a595b95be2e84bb1e302e63a8c143162e7aabd5ab5160a6bd46241cb9906202583a6c92a99a90a3af8b3fe02f7dc7a62bd37c82" },
                { "ast", "8831470142ab567295ecd28c85224e83941acebf4be1b7fddaef46b7e863c73050b334d6b1ec2e40eb8c0ef0a173cd12d36f91edd05fa8f877c62bf5904df786" },
                { "az", "50e8ea493d537de5efb9feca6c28a2877a42a006ac664355bf39b1a184f55727e8bbff1f56e6ed454d1eb1d9e33420c4f5482b37887207e160fc79b70d00f152" },
                { "be", "0fa1776e80dbdf4b915558ee4bd1a4cf356ff716039297ba212a0c70af6996f6ac260b25b74b3aa902eed29548050ffe86b4f2d80b587d733ac977d11276a050" },
                { "bg", "bed233020c0a80f0c4d169cbde4bf88953f579cf67699a9e776348dbd8e9fb4f50d63916e5909a874715e5374188f3cd1181407bc650ce54004f751a37bac3f7" },
                { "bn", "cb5669ee32b03bca51f09d89273a54c101ae6129fecf59419dba2afdd07ea3dfdeaf677e924d610f34602ba58636c57f6735e99fbb8963274753dad5c15a636a" },
                { "br", "497d65a2c270fd489a26a6339b61c3e38e544588f298896398854a24c97f44c0625e07d091e51e18fb70464eb6efb260ede43fae72ec417f6cb3b37ca6b27cfc" },
                { "bs", "c00790f11fdf56b06aa4c9d37b9668e3c73cc7775c93c7abe2f7f805d219ee1c70be68d13169f25659a625c2eaffeaf1837360838f6ca4d62581f3c3bda28c4c" },
                { "ca", "5b45a2028c61ad509f7cfa4f103a64aef986f23242b5589547edec86ee85f397f0aa6f5b98e92585fcef5438efc555888d52032eb04411f507e72c91b8f4a694" },
                { "cak", "a74969570927ef6da1221b21447e29a188b611da5eb6dabc9c073244bf6d6eee059a2ef3d0a788d199510178aa4f33cd178e4c4c7278238be7d3ec88a7ace92c" },
                { "cs", "bd40dc8cfd01dfc844001e96e89c0d8dbcdfbce2436e5f888b878d3e67ce3b24d0adfad4d6f422e48c65defc5cb0efb41ebd84b1113a519fdb888fd0987ea198" },
                { "cy", "12605212ad8dcc793948fa15e0f5ea28f2832b002905cb0a58ea8aff7343e648b459192512469e626a843c2216cbfff75b52410e21e1411fcec9c552414d81c7" },
                { "da", "3c94e9dcfd9ef67de839d9a7ce10df2fc92d7cc968b176c56fd84963e6e87a7808354fdf8789f8391c725350b4072b46f772af8f77dfcaf3665d8d8cff2599ec" },
                { "de", "f2ec494446d0cc92c011db733440187d45ff4d0d65078892b71d203b7dde93d7ea36267e9410ebf1c75c882d07f6774286182f1843b2e739a826fce0093ed5c5" },
                { "dsb", "8be973d2334cc8eeb9cdf255388ed3c645b467348b93cf36cd56c397f8718cefc0262f218e50a89267fe52aa3261c315e987c19e7eb87c30219d8e1c7a3d529c" },
                { "el", "33e2cef271afa484a5df7700dc4115c951af2a9ded0a3f4f378bd2816480fff1ee75f18dacc3e2ff60acb995d089ae052af50221bd19118d167f497c9c47f821" },
                { "en-CA", "017525cedb62fb78b35542876df7b743198ca59f096dca17ff22dc813ca620bc5cce3a59fb4cafa535c3b3589062c0939fb65456ba4a571afb3a09f2441487d5" },
                { "en-GB", "40cab4f7f78292c05992796c39bcec70df5e9f70b5b657307973bdee1a301db031b505932d85687e981cda6338afb4b20f8a596d877c0ded2a430606c8563c01" },
                { "en-US", "00a9e2ce962888be25dd4ed538b22d7de841382c74de96b6cf6333a61b57c8ce0f574b2737345b2983930bee1fe419addfd33d5b2a657a3e00a21653c165cc87" },
                { "eo", "a8f6b3b17a51c109b3ad3dc9a9b4c756bded5be0f75f81eabe950835c4623b4b1e1b29b95e84e460c2d35502601bdc2ff52220f8c80bf89d88fe01f48dd8a949" },
                { "es-AR", "70c7f064d277b860656f0b11b335c743d38404f984c3ffa05f18f93932154cf3727da38656c84f8bd52c9f17969a0833b92f35d235078848d6e10879b7c064e7" },
                { "es-CL", "3009948323ea7ce2f58d43696650fb62f1503d53eabbe21d5b02655983a4af4d1561e8029a0e0134b0f941669b4613ddf885cc6ed1c50e13ae100515483c5527" },
                { "es-ES", "4d43a7b9a27eb93deb86ee9a23ff8e1442da1bfc0b45f3c71b6bc405336aaf945663478d5582e857c9c238cc3d0e35182fabccd0504be5634b735c68f6cc4a35" },
                { "es-MX", "a70f3b9ad1db2406741130507fe7e41be11a495d11af017d186b45d77fcfea5026c61ab79b4912b9a04e7fbb982a578033373404ae995f3683cdb5c11278dbd9" },
                { "et", "91b1728d85d28527cfca0531c38d0308c0fbf485e0fe9db5c231ca37dd5285c20cf5a9181e882be3672cf2d8cf6f4ded3fd053bee932a04bd1ed6c6dd8b3b893" },
                { "eu", "1f6cc5841f90940bc839349c547d1ee1ac12e9e679c15934e287981ed663eec8768deeddfe74c47df0bea805175eeda1c251e11af861e9f56229334e7cfff7a7" },
                { "fa", "16184cc70704f3119a4ebbab172b9d0c3bedbd8c953ed0a72ca261e73cd419b05c5a58cdfa0b223fe8e66495930836d7143a1ab6acb80bd09e204364cd40482a" },
                { "ff", "bc3dbb19038ffeac1beb825254f6bc5d5f49ac1eec1fa0d0b3ae30dfa9b93548448375ec61888e8fae508f89458ff2eda6c78436dc7ce240aa7802d5fdcc6bee" },
                { "fi", "c25cfd2c75a881859b92cb374450d13005d8a7a6e5456a5b47c6ab5580ee158e797d22ebe2fe5dc4385f4582e3d6d6aa93a8d514be7d91d7cc26f5c57dff1139" },
                { "fr", "cea2c458b5168c8ca66d55da94beeb8e489766da1025803f5d359e5fb36d38b991a7f56a1d848b5baa3453733b18bf766a51fae9c3e03134fbbc407b10d35b01" },
                { "fur", "afc31e0b7d533631a09eecedda38d3bfde3fb3d6d36dce0c49d0a65c1ea352fb9ba547859fe9f7e12e30543fbfd54f88252c8f816c35ec39145033a65cab54ef" },
                { "fy-NL", "0efbd38c4e460d538d6d22ee93a424477b95c35e949938689ef6221a060398e2f6ce1e7dcf4e1f68035705d5bb36bc54156318ca93fc96b76b5eec1fe9f21208" },
                { "ga-IE", "dd5a8d3160e17d6cd7ea3dcd982c76805e978034ea9459906aa67d4afda2f74ca67a1c5f2fc7b76e1b3ed46d0e90fc8ff6f3a4e7f80b8ae8a74c092d9258423b" },
                { "gd", "c0590484c79a210bc5424ee553123ad0265d743ab0ca97dc88dbb039a79e731613768db38c5b9e620926621a20c0e2f40c663e83fc027699cac9bf94e9c93030" },
                { "gl", "4d89fa5e0011cde2fd4b64510d697dd19655a58ef456a9de62224060ab531946ec9c6a1f2316eed5d547a270ac7046c9d22c31f4a6b165a6cf962aaa94855f01" },
                { "gn", "535c8d39978c856f41fb6c741299c75abd9fbf7ae45dfb8722c9e679acbc677ee1da2c07a2a3cfb7dd0c68c7b41153815604a914c94cebebd39cde16da838473" },
                { "gu-IN", "57cb3915f7272d2ca166f3918bf79c8f2b2284f343a77b4561af120ba69e4b7dd6ab961e0674176025ef591d1fd5acefe4f563077d90c2054cc9d6b8a96a5895" },
                { "he", "35be838b06dcd62bd7ffd055bfded8477cf8d30b54a4f058880eb37ebbd02ad5eaf97be6e821734b6c4952d89d7bf6c2e9d66424c1ee10fa53178fc91be9bc26" },
                { "hi-IN", "1ce1f148fdf8e9761dd94c0d1effa1b944da3654b65aad731b28f7d4f08b2a6c8b225df8de0800ec66a82b3142ea86c7397547fc5ca9b9f09df39089bacbcd55" },
                { "hr", "a5cedf95dfa45e7000046bcb9340adde712443502bc965190cb5ffea8957dcc98b6695e8139444559ab72ddf59111506d91c2aa5a1fd97cd9749fcfa1f2ec952" },
                { "hsb", "7a791c749fd46e776cc33fed273cae269d963bd9300b580d14cd2859fa03e68ea9d42dd799bd2beda2355f93c54e816e4ecce50f439979d5809c6c49f2d7a691" },
                { "hu", "ac1bc6a8a9638a5c014f423b17d30867a00bc0f9cf8b3f7dcda07e0ee7b045c36ef550012125d6c3cbd42d18f7fb7f716369bece370d20c3f9f966ced72faf41" },
                { "hy-AM", "94d27244201e2d6ab0b0b034dc930b04b5b4b10073e7a12359878a30d39d6523f697c22f38e9cefe89da649bf6d76f44e404518cb8b239aca2eed8d26d1e1a1f" },
                { "ia", "da5cf42d09754274100bf5defe61784997f6c41969bf8b531450a8f53ad9db66ada9df7c4f986f7517484ab25c5ba7fb5081f10f3a29fb8729caf0a0056d5e7b" },
                { "id", "65626fdb6b949968abe946e3b23d105cf60ab7b4637eb9bbfdb8b8d390d7249f90d350074800e27e6900ea16ea40d092e8da7aa61a1943c8b1872018a365a3c3" },
                { "is", "7e6a22efa5fda2f7938834937bfbb508b15c6e26fdf5d0bfd9672f4323e2aeafc16fe512bebabdf53f35c522d67f296a290da636abbbc0aaebf227a8331e45de" },
                { "it", "2e7a233ea3e0e38d71952c3763a4abe892b86442882e00576e5699497e89cde7f9ac2d3669da00f189153f98977cad5b2a84c8c46deae021524277d11b72c39d" },
                { "ja", "abbab8d185c1a92f8738ae0d6d094fd14916a596ab1261f5f6c98906c7b874521040cb8ae8db81b870abf07899ae6eb63b9db14603298e9d9aa7764a60b36a0b" },
                { "ka", "08af756eb8c71d16ed36365145cad0a7b85a371a2fd982ec1f663fd42727c885f7d827523c432faa091fe27bf62ce0d01f1bde4c0cf111a4bd94b8ed198fe464" },
                { "kab", "6cb4f2332f72e22c1e0d6a5022c96fb11b958d2fbfb7d5884c815ff5a32d3b3f134fd0ba5c7ec8241bff7d67320e213f38f59336a50c8c1fe6da437de3167617" },
                { "kk", "74010923923a210b221c3f38e945dbc382191b3ba38b80215deec382d4fde119352e6c8bfa8b5a0fb65851645d166a64e8e41d50edcf1fd3e993811f5ffa07df" },
                { "km", "94af3fd1a8254629ea3032b3811558c8ed4ce369d71ba8c9590027346a11745c2e242d9c9df6230b54fbf9d412fdf70e3301566c2fe794faf79577dd745f135a" },
                { "kn", "409872b480c53ea852e0f06a51a71dcdf1daae8bc6d69f82768710d6a24b85745d0d7549479e304034222f07c96d499677d6eaf67bb65b90f8ac045b09ea1f19" },
                { "ko", "f9eab28ad2f0e1ce1365ceaaf332c0677f275c349745c0731abbf68eed41e6fc24cf40c95ac4d4e419e51199629506c5f65e572e7a220d98aba90019cf3855b6" },
                { "lij", "4858d102f0f846f3c147d728d4200d31c597e0826bc9bd04248b7fc06b5ce94c6d8bc0577d4d97bbfdb60caa7b47502685c40449eb3be21657d54e4c30e3c198" },
                { "lt", "aa2dbb50cc958861bfb227a26cb3583852a169a1ecc657654435dd04f87c28996ec5b5d5e09386bdaf659a2e141816372ababb501fe0fb09ae63672b858be0db" },
                { "lv", "64f14e64bd1d60696338dfaadcfc883b4624552068c83e5391ab2bea14a18d76a52b4562cb3b806606b4027126fadf9f5cca9dff16f5cf3462f43a4056cedcde" },
                { "mk", "d9728d02155bf2a055ef0dc37e9513c6ac07fde7e5bbca14405a412339542269300886a326bc830c1fde84b7b50a32b0cc52752580c682e53ae15d9f3ae2fb15" },
                { "mr", "26b5a8b78bcafbf7f54da19efdbc1b33cd24885402dace4d337681cb451b1e76a0f083846db917bca3322e7ef133c88f3ed2085fc2f6f771381f62b2d7642051" },
                { "ms", "8aabed741ea0b386aad4ad27b632f4f4596f50e4d6b5a520a35a9dc0785c58690f790d240f02dbe1796c32170e27a855125d1ee3712b313ec5805cdcaeaf85e1" },
                { "my", "9ebbfc6cef472cd7891391408c3e79b5a7467c5ce496a097333f2332bbef357dbd0260bda042a1d1593dc2fa7585f34e2d3f8f1f4a29b4d06a3a63608f41b9a9" },
                { "nb-NO", "2b6911fe5df4bd72770bed06a0d4626f43fcf9bbe45ca6cf60a67bed41657a290851686b412d94a1147b288a93b89530f75980ea823b1c81e993a05668628e0b" },
                { "ne-NP", "bb36228473402f3816a0716c794c1c0e848a72c1d5814e00efc4da81d3e3b6bcc8c2414477b5439b510b921a64ed2102e52e97e5405f17397451e9d682e64cc9" },
                { "nl", "1bef5ae627bb4bcabb28f2fb9c318ac59b1cba39991d8847545a64676ce95c61cc999c4d48214153889d85948c0df7ff94ce1463dc9c034fc13f13b12523357c" },
                { "nn-NO", "f7b83fe081c5156121d882623fd9c5cf4cba6ee9baaebc8db6fbb83be0b3650570f0405b799e5d0a288d2d4d8608478bf717cbbeb2ce2153120d58361854cc5d" },
                { "oc", "8debd614497c83ea52fa1de22ba63e3675868db97821e1171ca72a5aa358f0af9c966850e9d8340cd115f405a94b7abe1932cc46b91a4a23f414a964450c89c5" },
                { "pa-IN", "ca108019a80bfe4260b287e4705425ef200705492838582716d8024dfc4cb76e4162241f5526b06fe4ca804e764e04b193a084b76f89bf7198ba7f3b869e944f" },
                { "pl", "0e55f85a135bd72265dea0b9a06f67dd30b138346bae91ee44dc3b6151cfc8fe7545fe8ea55318b56556c20b83434463441df09c4d6c6bcca9409b91e6665256" },
                { "pt-BR", "247290b4475fbe6a3b979a0b56a4ce597ac9266984e4135c33baf0c20c95a8a7037fcb8ed60c4dbc1564aad96c83e900f9cb11be27c68a56cb10479a82d224fd" },
                { "pt-PT", "8048109ced96aa9f312c7dbca0a861b6a03cc7ac868c0d8abe72a5b3bc873bb521f4bf241104d00aac81b70eadd66a461e944d66c9f0b625d5d67d93e7f7c8fb" },
                { "rm", "1b7490ea96972621a0f0f3136f741bb093528d1facabc127c7113d883ed8f7cf76fe4efd9c4df986846e09b8c50d361a62150678ede3a67c62999fc5a9cc5886" },
                { "ro", "b29a9a89689f8fe23ee56361b6159c9512ec3bf1d06df9ac554bd21e3815395edf4094c283c4e1c031e2bf2a06edcf6be599b8e0bf95bc94fab89591e5268340" },
                { "ru", "2fb6d71fc56aa8ced2a3fcaab7a93a09963a32fd5e9accba5c336a2a4575491e90df25c823ffb8c3f8707668116f06b2919fe7a6bdf5cbbe9f336712824de416" },
                { "sat", "0aba945a07b7ce50fb70b97936a45eb77e25c5f5b1a0e2f03a425079e8fb938274f2c4197068fc12fa825bb9e504d40e199e7d0e1bf7a1581cad69fc31cdba7e" },
                { "sc", "3d48409af4c5cb4608be788378f0255cb537ffd9760e305e7d41f53cf6316df953834645ac332f51aaf90dae16ab62e8547840ae9a6f5b200e70fab15ce8afd3" },
                { "sco", "c897d1d0b79c40cd07a1b71685f8d6412c9947fea99632d5ee769f451ac7905d31780e8a0ad801b174e3cd5a5dc44ac2fe2839696a412aadf3ee71de4ea62359" },
                { "si", "58752a2029c916a89480be659612a2462dbbd62b83d86b7d4298723f4bea69bc89b8e1f212717aaac56683736363086bb13da456238dade4add52b0071358c80" },
                { "sk", "eeb831c7d9d64b80e52c59584a74fa3b1a3af600b067fd91f7ca4d6bf22077903331696a11f2365799ae3831347862e74ffd411ade9743e29aa4a765a893b53c" },
                { "skr", "2349ccd440309892d91bd031cc90237a5c85dc63239f18c1b08500335b124717469e37099beb570fd13d062eb3150c84a8d3983026b2c09084227c823f9fa2d8" },
                { "sl", "2d5b0c204dfc35d1c6a8d283493e311c28d71205de96552a7f254489ea3c6dd1155fe3d2f266f162aaae32018fa71268803491897f82ae2f33c05792b0a6b280" },
                { "son", "41ed4c1224314a906148928cbc941820faa9aeef2d276c7d6ae5a83a556c116d8fbba54cd47fbd21a7a2951051d2e42fa23d4eb037fdb6a5dcb2a9b6d3f2efd5" },
                { "sq", "4b9e0d3ea9ab102bfb6443d6e313fd90bd34c30294e304c8c676d54a937d6cc6ebed3aaa1aef43f5e4a37e1a52b146b0fc811f99ee025155ddff01a727c96af4" },
                { "sr", "1ba883f13015d38d43662020749038f2290e48e614f2c8d1a26f35aa53d9baa4a5c9956213a56411479e9891a5d30d87ea89681f0dc1cb273900a9d1a7befb33" },
                { "sv-SE", "b80a3ca1cfb92b432ff6bcf5ff330ce206e93ae63f8259216f85c077e7e95e7c41c9bd91b39d1402476f8bb7fc089c238a77ed8270a54b96f21d656c938b3265" },
                { "szl", "9a64af4507803fff477bc07bcdf5f51c48a94d3c3d0fba34c3bf0aa0a7dca9ebaf40545246aebd36d0984af6ebe03365716d335f8b21a5f9cd580d0ebde70612" },
                { "ta", "b6995c70e8ee4a8088852dc0604a482eb35c941cb96c22fd25cca2e807154cfb8e81f8d6b8bc97985491acfbaea4c82a2bb8b677c50dce212cfa078dfee01c14" },
                { "te", "739c5a04750a6f95cf1005b272d0a4a61b47f8208bbe32575b477ab9bc0e6c99aa159370acc951f1008301dd865de47ea8edc5c75b79146e3d66c34c20d0649f" },
                { "tg", "f13e4b6b7a843ee14dcae9ae219d6041598a9e0b81d1591c81bc3851b80cf7d39c3d2e6802505ee5e65368bc04aa21e95c506c2cdec7d11ad996e4229d303165" },
                { "th", "6d40998185f8d6f68ab3be4f701ca3737135f28540bece8dce83031df01b3678dbcb1687cd76b7a88628f3a02f0aef8996c69d7b407e1acdbda105a6e1ad852a" },
                { "tl", "c4ebc3f4282092d7433c918320e99de5152d2d409db3d0fd0a99be63d3c3470226c6621431e91b67b04c293da71a8ecaa403cf8987e47b764f59bd97962e07aa" },
                { "tr", "22991d6f05d36aa5d1bbb1178782fb0d4ca001671fe7b4f055d494467439c0f44182486bda3080b20e382c2d47f40e547d0316fdbfd493b97ece1cb8bc026b25" },
                { "trs", "63670eb9aa304c967f5d3812544493c04a73590398dcd43d98479eadea8d125df968dc2eab88e5f0a7ad7d6b00add08497aad8cedf5afdfc195f302882e32005" },
                { "uk", "a07e0e4d933bd0aaf083ce13e43c68f833b36f702725501ec0eb24c6b2b7fe819ed3fdc4ce6dc69db93358594294ccab28477c77deb5f7c968d71763948727bc" },
                { "ur", "1cde91545f1ec218453c8ffc8de117bc8f906023aeec58a6f20ebea61b732fca5d8a686289b9177457f611e0be901ddfa64a2314fa17da999e415935d624b119" },
                { "uz", "d9e364860995e698d139561b5902d2bef1a3f8edf4b512be937fb9b514c9564d6978bc3e2a034870764906a0236561bf537a86b1ae4bda933b32ec7969b673e8" },
                { "vi", "11fd96062d3834cbbf4674892328a2bc25c252585499def425d50d26fde359312cd72910f1c895afda95dc502444f3976271203b97d30c52f88bd4aacb165b02" },
                { "xh", "342cac160cfd65d60d7828019226601e83e78d4f5d416ea38911f6c1ca5ca7d2763715b725739aeee3a789d48bc9a1464868896125c01b2f861efa4536fa0e14" },
                { "zh-CN", "249b3f7c23d241ea421b627dc3215ff68d30dc0854360917fbfea7978666e0874235e322275ec17cc1a849f765f97fd6c7288289447cd211574434542ee69115" },
                { "zh-TW", "1397c9e3498b95d4c1e706f6fc69f320bd4d851d48eaaa8f883ff09db5c45d95829ebd2271c5a29c07d51703c2b5725cdfc7b9fb6fb7083362276806eb36e786" }
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
            const string knownVersion = "128.0.3";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searching for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
