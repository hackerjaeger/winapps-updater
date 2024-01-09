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
        private const string currentVersion = "122.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "ca11e1e1b51b2180aa7b2ed685c4af16a67a5558a075e2cd0bc1d6de581b1d6658aa66c3b17b6d7c9829bd9eaf0d3b715e5cfa170dae067d8d54a6a1ab67cc5c" },
                { "af", "daac6421b3bd6cfa222bb46c9cefcc538ac445ffa6fa0362efb5653f4983f948514576cbdc8fb9d66d936da44da4f0d7438686ad178ae861b1d199d82191c5d8" },
                { "an", "19d480aa239b850917332381e7c292438989fb8b05e065280e054ae2b3fc68d5847f0ecd725a699659acbb9c9d5f143a3c2a9ee37f22bdf2a0ceff131df271b8" },
                { "ar", "bb72646db1f186896cab788c0c349304b56f617a7aac212fa283cad336606459f1a4b972940a88abe54654f25aeae05ac28bc4c3910b58b45e60cc7843d46ff5" },
                { "ast", "06d5a15f3a2f44d21dbc47cd1b90a83a11912158b8e758f58362af86058b8c582f7009daec81f025b3277970ddc55bdf6e0d3671deed74b5e98ddeab522ddbdc" },
                { "az", "c358f256a1242ba95c092a0b67a60410c93795044fc742e91f0c4709e9e449ffc0ff1c2b745819f2959b343a7f0d691ad2f44f30b29807b638d375c029bfb588" },
                { "be", "1b49f1f0e90faa2a2c1c79a786c27fd2bbe16ecebef2836d73c9fbf3c9af674ff57544a17b861facb08935d992a87a1599555024cc817d7992282fcbe469efeb" },
                { "bg", "a9612034318903267c8cff246708fc6bb5a3ad2aa170f29e0ffbd521c791b56e4292cf3283a6c6a302cd47c93e0beb4fcd07aa8a851bc20cb2bf7b223e86a092" },
                { "bn", "3124dee689372c2f06b87421d45c4e7a2e7855f2c2235c2e518e38eefcbbbb6ace9fca350484a78d32b7f8f7b9e2f788f01be6c0dd99c0152b86a9b2c9ca9a3e" },
                { "br", "b52e22b3c4795ff1292aee4a6b4977218043e908ebe7f8462d765e0bed9fd2ab4988a40e70b752931c921486e78d7db675285f53bfc06e6991f4b2736974dd10" },
                { "bs", "4d864609548f66868e1b9bb6477eae1b324c46a976335aaeacdaa9e44c2a0522a69e942ac7dbc33c52670965953d462211e6256938678e068605b80b55d4368f" },
                { "ca", "2f4771096c33f365e7a1400cf39b3c45553670e8baec6849b37cce5a16669b945afcf1f8f4a4c80d68858cfc9d8e5c76fb3515cebe5990f5e5fbdecab49d69f5" },
                { "cak", "7889b2af154749a260cd97cf5f3852a847bf6c768247e07cca9a3f83fd28fd37882fdaecfa6f10ffabd2f4e0ef68c781511c303abb1761708476847930ec832c" },
                { "cs", "44dabe8b81411949f332da8842af45162c3fcbf72cdc584ca71b7330573ad58f227e8fee0c0b68b487044553e79696648c6bbbb02d9c8251c299ece66bfd51b9" },
                { "cy", "60c2709d5713903264e117eb539f33b5c5b51a7755caa8f6048331d72d7647b0520517c7aaa843dfeb022b894b244edf8d541d18eabec2e43321aee0cbe235b2" },
                { "da", "bc4f8cb544a944ca0a2cfa455d560dc419b6c227845f6e0297786e82acc6d81078afeb2bae98b3b7595579142f0cf594cac733c37800e3267cd0f266191d3461" },
                { "de", "b9cffdf56208fac3fe691897b33e705b197d2355d5c47da466c17ddecdbd462b6ec3f43c8a71d1f0ee6e6dba9b3357f5bb86d9d96ea63e6ac890c9915e7a33ce" },
                { "dsb", "d0597db087f9de7601237a884e177e2a1eba8721ee07173052030abf8d556006ba7298a19d7b2294c235ac2a43baf1edac7dc9896db778b61b6ea1e01926c628" },
                { "el", "37a86ca5b67d32ee2c56f9a9e2b5f091538080b62986ab02e700ea0cb4d55ae443b47ca9501d70d74fb56957c12dbd708d8d012843c4a34c2cd1bec05c58a00d" },
                { "en-CA", "4173f5d62585794fda82357f4f533b605c23b192c0acf894185059a4d29792f66877f2e050fe9a7ed3974971cf60b042afe61ca95326e2c1f1d398f05d2320ac" },
                { "en-GB", "d700ba2803cea14263e7560cfd0687ca4480189532563d265ba4a5906757118a5a3c8900c73fe8f27959e633d0c9407485d24355210b9981e7eb41c410341ef4" },
                { "en-US", "c82cec1f2908c82947ab5218d3e055decaaaf5b6cd42c0611d70ae739732eea33f80bbc1bb3584c0e57ae0493e9ead3cc4b1b4f3f4f8b9f66a4a637a9bca85b9" },
                { "eo", "92b5bc8812e655e9b63d8eb74df1f3cc8a4e7d8619d550b471b931d30323776e7a91647b86f0e592aad36e72efa8eb85a1225488ee4a09bc94151891ce1538be" },
                { "es-AR", "6cefcb50bc143ab9783d91769829be40c463994765d4827a0bc832de95ed23577f21cd8df71c139a970404fa3ebf5098c762cee7e2895c99c1cc8edcdfa37cde" },
                { "es-CL", "2baeee8a7b49aa68eaa1e9eea296416816fd769eef606fbff137ffbd10087cc9aaeb113872fdea9f27d792807a650aeb13c1edf2cc7dd6cedbc7026f0393b480" },
                { "es-ES", "15a6e7c2c830af89f9088396642293039811b83afeb7ded0380da1bc942f247de673818d8f12f3163e6fd515d50e0b89d9ea59e7667f254dc91662af1ecd58f3" },
                { "es-MX", "9c33345d66e3ceface68b1baec35e64c0b42891504db4b6b55fede7419947072a32560989b16d18efcb67b8faff5aadef3badf239ec6813a7b01c88b46090238" },
                { "et", "97c3a8c8ad6592524ed79dd15eabb33193b5597c24f9bb2b2fe7a94aef5d4ba202f8d16d5b6ac7341578cc1c476571fcae1f9c69d2f5157c801cb5a1cd288c52" },
                { "eu", "abf6bbc05e193065576d220d813133e9d077dc591ec75b9930ab6c59ab95b046dbd456ff1d37177e6442f6b482430a224747261f76455954598f71c8c1e9c939" },
                { "fa", "7fdb7f1f558865566175fceab7995cf8c13515d5a5c850e054582bbb33fbb35b632af9f68118b3404da97249aeb8e966b81a616bd08e8fbbb49263900ce2837c" },
                { "ff", "d6f67269e0851a399d1ca27d6e8bae6cda352f904db650200c500ff7c8665fdabd4ba05d8fb9da4f6aa74a761d896ea19295b05b914143cd2e4991ba2f517081" },
                { "fi", "f885b0115793dbc5efa2be380640b629e7b0ea576d99d36a17eb0c1e97f3f70f1e73242fa3f96eeddfc934653810c6ff9b7bcd61724905c7bef1ea26e83728ad" },
                { "fr", "ddcc705e54e87677be77ff27af4ed9cb98be82bd38d47646186669c66b605918986df9d220240bc4d93a1b9be853b26e3fdf9e7777d755b83df98ab20287b51d" },
                { "fur", "b9a887ad2deedf42353bd2b3af840e196d8f434ac3af2bb69b1603b265ee343a53e30ebee483a9c1ac59c9b22be3861a47101d293d4f3231a0a27e4913f5b25f" },
                { "fy-NL", "1fdf462acff6e5de2d841d79ce54089095540c21010f13f9972aca51b81ad54461b1de9340331ad76b366c969385c8a0db6a98c9a7dd4e3785e4edbeca0f21be" },
                { "ga-IE", "5ae13f6e469fdafd56f474b0173e345df4b2c1e33b6c0b183d3ac1a4fce7f323422a28249164a83cd1aaf506d0a6b000554ebf5c173cd045a8b050484bd2b151" },
                { "gd", "46b4e1c0cab509777d88df9981b8dff8248f37921b953fd22064fbaf1299d58e4118bab8a7c94fa8aa9c6784ffd8cbfeea699d3aeabaf269e5ff93014ff12e13" },
                { "gl", "63e60c45682cd6003f9d30b4bfb3e32a00e079e6cf509538156ec03eea9784323fae0cd0d609156b356ecf82ff862b821f3c34580df03107a0fa698084dfa480" },
                { "gn", "bc0a380e5f92d2661b11ab12f8dc4d482d50078e88b930130e8d6b0300c8726fcc6b1b6dab6e901ecb3a5b51ded1fa917f559506e38f142b9754c27350fac8e8" },
                { "gu-IN", "20e3614bde1039c3a6079d4cd5afafd8e4ea47560792c45424a4cf5861e0589c9d87e72c93bdec7e5ae7bb195d65e417d95c1cb1a9915d72f09aee1c34508eb1" },
                { "he", "01e278f1f50996ebe2076cc881ac46aacabe9952347a20952f024abf11bbc485253f8122f497f6ca870392fcbba09f06a3d5d0ed47c856af2db31daf1bc88722" },
                { "hi-IN", "98e97de1c21b11f41e04c0ac95b23bb92c333beb7a45c2f74223c7a94be2193f0d66ffa5d31d94c704851d6e2d8baee612ac27691b21ce91be535b998c52e069" },
                { "hr", "a67a89ea8ccbc844fc01465cc7529c91eb5ef8006144303e7cf76536d62319b0201a0580fc854515ba21b653d93a439a387561d9fa55c6f2d855f91022e297cd" },
                { "hsb", "7c10b7997e31430b9ea0030163e87f070bcb22005fdd4b2c624d693d24bb5f3f9bf12dbba4f2a89738c02f7e258a030d902ee737a7bbab6c5965f537c7204cf3" },
                { "hu", "b3f8983aa46d48424c83f25c3c3239e3435b3c7c72a11f557a4d2c982703ead8e101c85f63595d8a5df0472ac74cc6ecabe7eeccd7cc13cf64f4992cf072ccbf" },
                { "hy-AM", "4af1a84f7addaaadf129db0ded6b1f62be58d575c4c4694c975674532038406d2fcb1fc0c61be79b043f383f76c3c35f8faa8d5bb17fbd0048dbe58a605e9bb5" },
                { "ia", "ad59facb29201aac274c5fd642a06c7e0a991d8a34786a8e719c4ff47407db7f12686d4fa92aefaeb3b9de7ae548062d86a935b0b8644c6bc1e5b59cf9e7288a" },
                { "id", "e66c3919c233da70d9f13d6436d38800f71e54fb65c2d6d2ff4cce0c24ba41a0c81837ac8e40b8ee871657b16119b4a5968437e6cd2831da02b2eeb65c5fcf45" },
                { "is", "435157febbafa14d5eaa9a442168a4ba8f3b4b9db6ab96266d1764ca797c2275e0b5d2e75c71143df56f45a7d263fd2a01f5a766c387c64210c9c94e244ed313" },
                { "it", "bb4e9b11e00fdc213835eb4ab41f4c03cd5556f93d5c4c82437c9cc548aa316c9d269d7234f7436d49a29957cdaa7cb944c963608f15a9e9a05b4d941beb0105" },
                { "ja", "0132511ee998aa5af286f524dd3d18d66529661aa09cfb87d3251c75018a4042cf2338ccce4aff6f754510a11ccc8a4d3eb57112fed8884ca4156148b58cc97a" },
                { "ka", "c9653df46c2e79661dff905cfb123185530ecad2771908bacaceb8f04634d64a0e378744ff856a68994cbaa057dab2bcff89b0e6272b246e4439da6adaf4213a" },
                { "kab", "afb411bc3cc8d14258dd064a580f0983279146678de8116218671add5b4637d322ac07b342557072fc9d52d95b077d1657083a6a428e25722c1b348ddf3e6913" },
                { "kk", "c824b8e08170c9ae0410bb1d234f1b62d5cebd1b63230d09e34f1d2506bfee9c758f1f7ec882138debb3ded43c07a55da0ed7b26adb5a53de14f8835bb5d5484" },
                { "km", "ed280ee099a107ec1e6fdf1edab6ddc82fb0c924e62ebae49ce18d65cfba0d900f6d25667788d9427aceea1b6d190eaca17143ce3af576acb4e53aa515e713d9" },
                { "kn", "e51646bce7846389ebb20e4777b48f6ebedec769711d30512f1db7e4e5c66ef8a3af5dc06ffa1849c9349d51e8e3b4df36b4f39ac675860ebba3ec63d44aeb41" },
                { "ko", "50ee6dc58bc71cfa7b8356476a8b9838c9d2d516e6702101c9baab0ae3c295d2c7417f9c3cb0008fc80d1af0ea77a7c02aedaa528827e868a87fe8980137e9fe" },
                { "lij", "fe1809a575a706e6c0ccc47317f08e47eb6f53c3d78be8b574929d522c83417af67cb4647d7313c03ed9a6a05d9a1148e8670827f1222860d680e9662f9e9da1" },
                { "lt", "f856cd6b09016ef315271671f4916c357ca97ad3ab48e34c7c2aeff6a626c70047e0b87f17e7dd9e6cd14c5f52bf0ba7b46e9164b8c37da6efb7b265019cd167" },
                { "lv", "0464b482f1bf1bcbb8619218e8185d16a4a849a23435fee56d062641e71c36b65ccd8332f816a4a93085934e645eb8fcae883ebc300266a3540484659573da0c" },
                { "mk", "631c1c45121167d653179f38508518e1a559ed44e42b55fa410e36da916cda1bf2012be2207f4c871d3afebfb822efb8cf11e6b5f62a5e0322ce4ad5851e554c" },
                { "mr", "98d0d636981daa1527daf8f46ea25a2f7dc756a1e148bddd13359bf0c1f640887984b6b3b1bd6bc4d45a85b455426bbd329f93b21bc9d17029ab44593bed8372" },
                { "ms", "98d26cbb1b7da7593e5a1d2889b2ee291e3cc0f9a79f0fa55a0dc7cedea6074b59855be51f37d14943bc3ad8fd0060056be63d3cc9b825aa7146643512907ded" },
                { "my", "09deb6c66aa5a398c223c910c0577abd9de622e875681f4bac4581ccbc2767a515e6a69b1c84aafd7e441e9b2605d97dc6427aca7e2ec6c0cc7fb060bd7bdb32" },
                { "nb-NO", "7759e767caed4d6f17a4e905a0e3410608ef6a547aebc9fdd5e6137cae4f7df2a36d62e702abd04ea135079125a2d3474162c4796840aca4bd59bd8e9965c40b" },
                { "ne-NP", "5227713e727b493a41bb5b809f36195502a1ac4d0ba85de8774da3f710fb6e5713b3f1a1c6eebcd078a8ac4691ab1c2e36c660825b1d9a30ab3eaa7d7d248a86" },
                { "nl", "41145b8b203877cbfebb4541f734ad8aca41f293fe472dc1c33eea6d5a93789e2b23594670c375797a3c71df9c5b754acd905496210481aa7d6f4b40afc22e12" },
                { "nn-NO", "0dc1ca59cdafe64a6c5b9a09dc0408862bd790b8ce1145cbdc3571c0cccc5530ac7c1e912c6c94b364512c41845485214e24a466f0afe53f4771e5822ab9e7e6" },
                { "oc", "1f9b50a40e852561bf18cff3d8da948a035be738796109edf088298867415227fa648e7b189b29357fe71237cffa2344550127a6bc9c84336fd4246bd817cbc2" },
                { "pa-IN", "368db7c6651a353fac9439f50c5345d9e65de427e5817321b8590f173d78adbf55a0ebfe9f4d54e8bab78b187bba078fb23c59f17e0d9f4ff7fcd9db00056b5f" },
                { "pl", "c10174d5373e40e3e2382feb03c3efb1755e8c004d7ada013c2e43f254fb43444413cf2d9e13f64e38bfe75222be12126f3fca1d34e3f7ef6bc6e8c55d20d071" },
                { "pt-BR", "2b14710d7d83ac7a86a7881319a73d3cd25cda05965e2dac11b9fece2677972cd92f3d223239eae4121c605288e9a367cccf6c850facd1d1731b42da059a2c99" },
                { "pt-PT", "6edc059de66553b7089de4cd6db19cd1166169dd062139499068939902f7e317fecb943bfa007aa4a5a99cbd8c5735d905eacf937bebc1a712563dd679dd467f" },
                { "rm", "bed4c8e2950296e6c3ea56ca604a93f4453d7db0cdcc79794fb92f00cc3dd2e6af8eba658ddd348050f9bbdd880adc9ef8f68193a26475a589db559c676fbdad" },
                { "ro", "6fff908825f5d3c203459dfced1fd4156066e502300bcfdb9dd81f13e054e12e4a58cc2c2c18c615e3749024325581ba567b0a0287c13d3734bff7183a325394" },
                { "ru", "4860bef3a9ec8327e0f70c7bfa90b4720369b35c48804ed8d6c81cf7543330d2bea613f70065337d2ab71dea890459dd9a9d5a29d77783a216fece300edc91ef" },
                { "sat", "6671beeea7341cf8774ac8ddb2e009d0b5e13524183bc4390a4d0b9a51472cfd8fb284de43bc92353946158504d63f85599bf6c3a22be3fb7a26faff75cb8395" },
                { "sc", "53801dec58d8a03ce026e778163afef5d3868883031cb4d3015cb637e21c9fac8fb8d216170c24ba6b90085c8b726fba23d8dfa04addc1281a6a87456265a2f2" },
                { "sco", "4ad1477307fa55898d15f4ffdc11606fa4f986fc7059001461100db45a527c74de50d84d36d7deed3df221c65d524366f8eed5933f283eec60f7197647481133" },
                { "si", "33852ea964d43668826e5980102286313f1e3f310519558879eebda1ced6d90a1d202c1047985818a6cd81459c596a7571a7672e49495aed4d5d479aaf4360e7" },
                { "sk", "0b8701e1b7ac892a1052cc22061d29a2108568d0df19bfe7484ff799d8773ac319d3abb85932a1a8a2c9375ea05d59377d41215dd07faf2f211cbb1cf8feda09" },
                { "sl", "460af3e83d63387f2039b53b8ec0d79f8adc1649d59fa064204a83a93885ea8b4f21a9729ca5c4b2b0be12b7f41e350750dfd45734b192103d5a5261f7e19d07" },
                { "son", "527cc9761bbd4b3d2c52bc17c46bde37dd36ece11689b636fa6a3e03250109f1e94f64f2a2d79aaed3b95818b1bea9a9c86bfa662d7e2279d68faee1323515be" },
                { "sq", "71e7660d6d94b0433cbb9fe75bc906765a5435396e0fad3a6c5f793b3b6c8ebed670bb685930def411731fcb1be30001166d4299901ea7083081e818bd135eda" },
                { "sr", "ea0fed1ef6d3b274ad3cde5a25190f4710674af5b8f7eaca27cc208e89a972d3bf6d2fe1f0f6206d7d770e380aa6c40f1a750afbdd6f46cdfa6fa39b210e91b7" },
                { "sv-SE", "1640ac609a92687f7b58f02421cb94b291a0d3df4549e75b95a052d5f8fa62818f4504a1d0ff3489ceb66fbc0fd13c3479d2ff968d77eb3b9ca240a01de7a4f2" },
                { "szl", "1ec046f79a586485765f82af2ce8ea87a05283a15c728f46831c5084aa861b401b3e64675f75c0853f259c8e4a275b117c39407d966d42e0ada639afd82acab8" },
                { "ta", "86ff37bd483d502ef7a5c79f0fb2162e495b5deab62a803f31d7f4eda87e7159b41e37970a04a646e3abc2cc293781444e1e2e2790770fe48d6a15fc9449bcc2" },
                { "te", "434963adff4149e70a0c7c268eb757c818d13be7ed9da002da2dfceedb98318796cc54bcf36b832e90ddce9ace703c519274b98060f70f5e37b8dcd86bfd414e" },
                { "tg", "87fad838a5d3dd352ce9594f46265dc91570cb34dd9e37e10524315e3de1aeb63b11fef37308af8071835d0651da03a857e33fecb20d95545b7944375d2b66b9" },
                { "th", "1ffdf7e847e4b522939f23398c55a72a10ba94daef8914ea71eb24ebc12371458d3557b81e67f40f61a87eadd7082472929b2c7c34d86735f524955304376593" },
                { "tl", "df47d9eb816f73c97fc271b575e5412ad3340330948433e647659d8907456b4334a60e82505ba6a219b244f4ac85c9cd02e96adb4b217c77a9abcf044439e3e0" },
                { "tr", "9d7b5a47ff8a5d90baa215d3f890ff527d2b49c161af8d675001aa85afd70abe9a5e132695a10d16cf4f1ef802354565013c445edc873279d7aeefc63b191323" },
                { "trs", "b1796a5cc9e8785fd1c8d0d0f395dd8929e4f6f70e77b40653053189e038b23b980aaae079cb584385eb639fe90eb2bf0a2c8a7f3401eb0703fa50ca8739077d" },
                { "uk", "6270b1f9b13f3a137a012a2c003422a84742b4679328abfaa39516bb121a76a48ff42ecd33152595262d8f78a0104ba9551728a351cab1a5db704c69c968c69a" },
                { "ur", "d5b49b7d1e97582e8e21d39b44fee4ba077f542b9073a5de925cc5fe79a8edbd43382195a4346fa073a26e861df5a33e532beb77755a6a9282a80aa400cbdf51" },
                { "uz", "fec8f92095d44135909da18e75b8d63309ee37c54fe921b3fb7caee8d3fcf3b67e60a5e2e7c4da62b5fa8861b00a2c0e2df3dc6b05c08dba863ed1586ca9cad2" },
                { "vi", "205cc0fdd8e47076cfcf3f3c8bad4112dc62ee0b4a1b94d3f79eeac8bc320aa1f6ad30944f3ac9e123a55f23814609e9ec73b734988ee55e84ad9097616d1d8b" },
                { "xh", "1a261d0cf3f82f1c2440a9c44f0f16ef5177377e2353b8a29f865ec144e7a5f7e24eb633ff65ce936ab5597f75ddce058d97fad512c189f0d41d863a908a3946" },
                { "zh-CN", "f2ec0c103efc5f5c660981c55d4328863f17b906a1f704c852d56f246e09869f8fdf445ad8f676e7339a9491995559c0212fdf9aed412495923376566543c113" },
                { "zh-TW", "2e4156fc4ed1e75143b59171770b071040e1346dbd1f784a13688d3bbd5f61be0c5edb1808671b536f3d5dc8a6ceb98e04ef7c03b93e8de0d8e0fc3b147207a2" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "3e75fdfd19b092453591d10076237e8f95dfd7f8f3f320f9accaa87d8b01eb3d029071a1b94f8519e454e74912fb679ed6cdb757d305abed77062e4cebebb06b" },
                { "af", "59730444ba1a67141a5d16a65a08d18d0dfac5ee8250978feee3703522d6db56c4420b0cebbb560b9bd5a4fbab2de1402700e9d4169c4174ddeb504f2f3133b9" },
                { "an", "1ec5fdeb3a3865fe886d515f92b0c1d2b14a2d25652d8bd52dd9a3084e1bcf08398db9480ee6c3d0eb7ca492564f6b70a1189318dd4b95aa06160d6e38c90e2b" },
                { "ar", "8775f6491106c7a343a603ec59292338f9e564a9dd5d9977749ffab83b82d9ffbf2b66f08d99a5838f9f87f64c741d6814c687ce129f308186e2f0844bcb4fed" },
                { "ast", "55a09eeee7d96df75ae7d4ee418bfac57277deac5cbc1f01c995845c94210b643fe0232fcd74fd0a1efe1f766de2b04377b5bdb2b9997e66241eb7c47af125b3" },
                { "az", "4532f2f298728d24d7226c22ce4e0afa7e49329a83d68c2ba227613b893006727001382b1b72201a638265977e85c71824fa120ce6c6b0e539f889cacfbbba85" },
                { "be", "d84128d98dd6539976935814c2299ea1f937ad3dd056e54694a0488111d8d67af518daed1120a9f321f4dfaaa5b01f658702a3f2721e869fa14890d8f1de9dec" },
                { "bg", "17ef185ffba7edeefbee5892e40c7956183cb773d4654d312e92f484610026af8debab993f7673c7ee480f0bef6bca734f48f2d8ed85f3ed78984542bdf7d0c9" },
                { "bn", "7e9c63087806aa38e72c698b50c368a65181e58add2b3b88cc948c7a8188dc9b14cd63c9d29f3b2212519b6d760614a8ce7cb08d21c4b7637062515b6d7f430f" },
                { "br", "9dbdb98c0f554a909f8ab22d8db7978ba1d1d41471659550d9e66a9a8abe2c3a5eeb83f9a0268d71b970218628fba8a6db5a999ca4db85c27757ed1bede718b2" },
                { "bs", "f0292b4e8dcd664dcb641c8707d257d70381a2f43b20b6abdfb7f2f1269ea39acb4f0fd4356b96e5be319358bae6608c33f8fdc2bd2da1c09a4c801bb89c0cdb" },
                { "ca", "b0bb4f70fe8aac80c3fcdc0cfac7caad1cd8967f4b08ec7c1d32281f1c27dd487cf35451356857ed863f972c62990ac55142542941e445c934591e4bf155410a" },
                { "cak", "791c01c3607b7064efedfceb90cdbcc59480d73fa0420f2e090b6eb03f0411e6f1a2d3a89eb6464f391e5de91f9e0294d30ac2a0052abd633126761c99878f04" },
                { "cs", "9ca963263c2dbeb885a3503c9de0cd77b26f3dcd21d4febb8e7c47835b096620c25d088b49cbfea575096c211ceb15e69bfd9ba5e55843d1b8b3f8348ed9d5cb" },
                { "cy", "e3cfb1b06c83577e2251d6708c9c5d4de0df8167ec227d3386fab9beee820703bdfdbd360c4a0b829997110c3daee4baf74e5d7ced47ed7ad6a8beb9365aee53" },
                { "da", "f3b52c2189d29ae1174684650cc2effabb62a3d0856017b549996d818b98240d863e6a12abecf08eb1184847dcea5c75f78d0a73d132f6548787a913e0c8b9cb" },
                { "de", "5d0c8c7eeecaa3d5689faee24374d132dccd468472856517171892d898952e11be4682d3fae611ac9820ef88da84b8a6d185fd502d1e2b0f28e5f5ed805aa96e" },
                { "dsb", "ec11567f8a9f91d3b2dfe1869ba248256aa86b28d0ac2cad37b8e270d18cc4a64c693bac820578936e23c6e02520ec0e50362c6e1fbaa0146f247b81ec1c2828" },
                { "el", "a21abcfcd40f1770375cfef487a271adf906159af37b62db6dedc5c5ad0689ae81f29658300fae99a523d58b3f733dbe583d78e14dbca16e5cb63336ca8a82f4" },
                { "en-CA", "9904de22c37073f20d79b55e14120601db555fd8f69e0205c172bfc842fdbc80201757d99de17a1eca57148594135d019ebeee1c3e35cda6aaec47bde9c86089" },
                { "en-GB", "a79bad7da126597d381b8178a2fd704f155e0ac2efa95fee90363b8c6cde2ab8f47a0a7344da063a0c9016c8ddb752bbcca144a7acebdb8003eb5ead086410bd" },
                { "en-US", "b9fa153688655a9b1df069d8b7503fca2396d4f352127c51cc84fb22adb51e2192bfbcba2323f6a45410248b2641bad9ecc93af5e5d0924041980ddd1da85198" },
                { "eo", "db08f7a37658a9d2e19b598c89e5446b463c90c2143c4ff36137f55eb4d74d6fc395b6f8823a4d4a30217ef105dfa5b0ecd313d7e2ce523133ceb47db374e6c0" },
                { "es-AR", "294c0e912364e6ae3cd09a5dc6c8a5eae483ea3feac341629783eb2e28950065e9b04d3340294b65001fb500b94c7aa9fcd827aef040de6a1e84173e70d178e6" },
                { "es-CL", "d9e6e62364c431185c67dab523096b542ab66f6ad1867bc77264dc8f4050445604d5c29989b3d90919d6951bc76435d90a45a5592226584044d07150a11bf65c" },
                { "es-ES", "29d84816221e8edc2cce30591d3e058694cb4b0c52d8893fc644c59f0f1d1858aa6fb276665ff8c99cc154a213ab3f106a682d5fd5ad3bbf29e8bb3ba1a316f2" },
                { "es-MX", "08c5e2b12312ee6c9f1c51559acd365cec66528cfb5a9392bf592f850cd34886443b3c98296f82516f99f5bcfef0d523ef7a5e3c39450336ef85d647cee63540" },
                { "et", "61c44b940800913e5a9937e5ef02546081d25bccead9cec14339836f42e22d365f39e1f1935e5ec607db80394d9a0de4903c2629ac414ac5fadaecd1f29f9f9f" },
                { "eu", "46ce7dde53a47726d2a298508029a034cd2307b7613c31c21297a7a2ea30e8beb7f6b77e1e75c53f33640167575236769c471447b955dcd78e965aeffe50815c" },
                { "fa", "6c3d47865bd74caa803035bc76058c9d3b1f1a74736f09c840cb64ee76c53895b946243bc7bce7bebd52075b2e79320058b98b71d0b467cc9da6ab4c590aae70" },
                { "ff", "07162a46bdaa9da8ffaf281d96d1337eb25442e9f4d0f957c2ce21cccff89a98878d452bbf90b318cae2e36326534bd31bfe805ee7d3a73cb184257504abe505" },
                { "fi", "acb3d67b8f87848f88abce9184b896bbb5d05843bd2a6545b81ff48fd21266449bb8f783475fddc45f9094286421be30b2d81a89329a89e69795a5924f52fa6e" },
                { "fr", "10919d7d4f09df1d9bcccce1f481bea6d4a0da4a2bd586eb55a812f8b5672b75da10b830c2a68336020985ff5ff177b1bd69903f09f248b741cc448b22e34a52" },
                { "fur", "a8cbf3dbf7ede1a285500a7643d27c0e303910737cdf71ec6df68f857b8b36c3a2ceef4f06441fa82a8701671c8fbcbaa6ee3d12613d567e4997c1b42a71dfdc" },
                { "fy-NL", "b7ae4fb21a7b7c7ab3af499d31ce29de607607ed0685852e01d363989680947de13dee489b7068477a07925ba02a49d368383e6e9e7396f0b9c3d3158c85e201" },
                { "ga-IE", "0525469b822607170b7b27908a80c4af72e41b06dc1ea88d4932652b7cef8f29069aff1428675ce0e101603557bfc3b8c4d6f196fd8cb5e7225bba98978e1378" },
                { "gd", "9f03cbef0daf9b036daed9b73cc09ddafe4bc8069b0d729e26dfe771c28af4ccac8ce21766c34c8545a3d3a9adb3cf7d8465f112d263a5033ba79d923010e0a9" },
                { "gl", "6c505af26f9c1b74cf89fe362c75aa5b362faa96d35443e7137202509c83460012e20d054af1e5bf594fc212c73b109723e2014eab7ca1fbd3d84871eaedf8ec" },
                { "gn", "703c8700dacb936ebbb11372284763eee644f428d1fc77170a32535d53585b05cfaf2aef9ccb7b42bed3563b1a6502fb2eb2970c0619211e38d527a82df83dcb" },
                { "gu-IN", "b2e6c60e7a20474e116ddca9ed5a05fc3249c334267a92ff5f5adc36d9da0cf96a5434f3b2d35a629392e1930c7ef06ad516fefe85442d5ca6ea302d40ff59c8" },
                { "he", "606c2abf66b645aa469618a37969bbefd8e57a81f996a66a02e3cbf65d0ad4fe4b2ff43d30998dd0e1f586051da6c3e756f17a2478b8a22cd9188e26a3a84509" },
                { "hi-IN", "2938ec78c35f685344d47ac62d90841efae6cd9326bbc7e71ea322b7a5d8e29c357fe96d54d8a7dd4bea9563d7c8a2afe3ab2b44a9f47fa0bd8c4c360976d6dd" },
                { "hr", "f64d8432435e1e196eda7d65078a43573b5f581f14d9dda307cded893f7f1380645064093bb3b68332c5566b532ecb32b492aff9e783414f4780293e36c10b67" },
                { "hsb", "aff7ad87a48152837e0c4082d2d5d632296e51947b01c5a4a6c884b5d0760170869a04acd2ac60b2b4076f0e2a6d292c09bd855c4dc1bb656774cde5743704d5" },
                { "hu", "43588bb6f359f827da080ffa117ddf543ff1f09439f4ea53f03f62875ead76bfc90c498fee15132403b0e5f728f4bf259cc1e486590b4f8caba8cd6b72a6135c" },
                { "hy-AM", "cbb74e471e0d45c0739e5a7be7fda45835592725c84313afafe5ce8fce5ee001adc001082ad0714a1ef6a5416ca795525ad0aae80187bfd1e53e43d0849303da" },
                { "ia", "610547af32490ecae10f34747a5d5a6bdfcaba7b681059f870227b8c7b14e9cc171fecb17965242672ec35c0fb70a0e656e1d5a2be08d9637c725e1a588b9bff" },
                { "id", "754e0187cbf9d696bb6ff803208f1620e3215492feb6809e64538e917e8e5ba20e181b31ba811e32d1e132248f45a26b7588f556491b0e27591ea1c17dda28b4" },
                { "is", "681fb527fb0ae8ba33ab374c56ea5ac941815b807a5b823c711cff862db94d965299296e1130e3a5fb5cc60ad15e00d4d961e86575a14ccc1e719919fbdce5ea" },
                { "it", "8e031c3134ad82926f799bc4cd07dec5b493a8b0a0d303c294f0327ff05a635e69a3fc3a67020b0a5e431642182a6e6778ffcc3a683f2ac3980e4e1594ecd0c2" },
                { "ja", "0d79a2e9bf9db15aa47d514fa68c4639fb1dbbb114c8dd68e23c20f06ab7a399756c20e95fcd85db23783da265183ca8ef51189bb51df9ea9eb14e3b228d1908" },
                { "ka", "7b0503754186d6e99d310fa613c05b6f77276d02d4540d5aab0b1968be85843a647cd4b1a3b087a2233e28a8c80e65f47f2792536616a4bbf8022e55063ee619" },
                { "kab", "c04b36694f036a8311a77e79f5cd5a72962b3343e76ed551b0d0b150026d3710d2e1c82e9619b5f3e07b22c74f7be37d7e52f49016276dc2bf8b0d7f4408eba5" },
                { "kk", "9d9241e9bf250a3e1f7b4ffc59e48a38abc2c9d5db0b9b76a0fa3cb36aa6f9267029073611f693514d95744327de92a8bd360b61a23e1e2cf0630e6fb6a1ff84" },
                { "km", "29e7e39ded543e72f4e3ab70172e82852670c8f64c875b4911a1de0f16bf83e107ca16a313b701aedd608a2b03e63855943628e3fff2388720a73e6f8d2507ae" },
                { "kn", "f311c71870bd5f322ea3fe506ef92cd4b97231f38e6698efb134cca51a8171f1d9e8f71e8684bafe09c2bc7e10028c50daa950aba8a052f9970509b660e6aaee" },
                { "ko", "522c8ea0eac7091a7fe245779e72a2b83f4134859eb6ecf8712e411240d36e4a0062fd2e8b1ee09897e5f96ffd48fadd03b72617985833c93a946f3b3d430cee" },
                { "lij", "48365bcc99c379548011c91ec8792f9d4ffb1b7bf04dc911bac8bf46e33c7839b6456a35d8098c846858d8730ee6f4ef420241391d0f7f3ac02aff858dee57c1" },
                { "lt", "45870b314454700e90d0ecffde410ccb565b6b59dfab43efa9d3d0eca5b3aa2387d062bd4713af8a638de284f6500d031fa3bb4ca6856c0ad76521e8196a2bd8" },
                { "lv", "1f707ddf12b5630c6ce79830cc69bcc2f3d86b254756528b977f986f6b15a1a26bbb64d781f2ff49e0501fc7c5457238cb4d6d4bcd9517e2b1ee230d0125b000" },
                { "mk", "7c823a54c6cb0a141a9a7f6139ae72dfd4b18678d123b02c812c284c77380e8a267327243f5f2f45ce3de197c9ef4fee47618476645674e95d53974446a1deab" },
                { "mr", "1d61892346589098ee9d2846862d66c934955eaa66e92b8a699a573008e1f95ee2b87b9549c06dc36c0deeb190ac52da6a108355722f07973ba65bacf424f4e0" },
                { "ms", "46f23ce3e5ea6ee8b9394af5bf28862b44c4edd0d921b60892e218302e8775d501573d8b4e3ee4853854f87c73275e0c26f1ffed285510524d9f59f51f74479d" },
                { "my", "59f3f9ab3a0eff66f49b4c08d3474a0ee0111016e0c81220ec3e6eb7479b3c74e46786f01300a50955589626320507669805cab2b793cc9f15dfc33402c87f96" },
                { "nb-NO", "547a312cf8ab87287cbd43b4d7a3b7521b928d7cee2316eb86cad29d1b13afca358344fddfffa64019ce50d70359d9150c69811589a510c9cfc41f0049d47878" },
                { "ne-NP", "c5e1d7345600ac5e225b43e3c9cfceb2bdaa616363762d11da9837625946f5f3acce8914a8528745b02c4cdd679febf4d5bfadde4998bba7ac193e035a1bb8a5" },
                { "nl", "c89d446a56b95bcc86d007744eede42b85ee43ae45e612b67086edb47dcf1f26981736a95d6e75062295fb5e20e58f3ca1753e5b93dd5c1f11756d8821d8e034" },
                { "nn-NO", "5c99454b43a29b4c026f675d0c277ee1cf40296b599a5e1bd9c65d435b8997c70d1d9957deec560ef6f198c0f37d67c8e75e0f3115f4c79d6049e05f07d2af57" },
                { "oc", "4dc04ffc51cbc405e5162315cae568f09f6810d8a36e6fdfc92917172635d36c346e3902f815b0e2fff43e5ac7ce6283c73bd48a721bf7b016eb4dd44e77d282" },
                { "pa-IN", "aa115b893f936a534b4bfef08eb17b083db0116860a65eb16075cc062cc46094a645c18d2693f2df76e994279ce89e6d75abbaeba04939b990b264d13a0f93f4" },
                { "pl", "8121e3df6426134228bb856ef4aa414e8250cc2cb200c083358a0975b68732ec1529fe9776297070c949b183a531cb466431c6c4da19462a0fb43d541364b95d" },
                { "pt-BR", "ae3d92d258b1eb034e5b9cbbcba995e3efc8050b609025e5ab71cbc20434da3f2ca6d2126a9322b5f017a877fbb3af06b411aa6d434216c8ec0e9acffc9f4ca2" },
                { "pt-PT", "818abbcd522e2836bbcfff1b71e830f68c8de21d8c9c000b03c33f75ce154b0b184d02ba96fe35352917b5eb020f8a8879215bcdba9925e34c7282a098093d6e" },
                { "rm", "a8d8d73bd16037064d723f46f0f185b0884580a2f10a13c50a994d96950fa553c9129b2eadf11af4abe2c4a58878d03c0a31a7fd00586f33c152231c51edb2d1" },
                { "ro", "1f576fa096b5cc9ccab141cf154c6a13091a6e3138d7780982f8e9e4fa9653fb6df1373369ceb6d03f51aca5912a1e87f5e632e38d3d8eaef3a173285c7dc63d" },
                { "ru", "54987febcb26f56b6ae1b72df479c6d5fe498c7c257796dcf542f3673ad0a73b1d2ab4c3df4966ef1798034ff095c7adf3b590f599a987779c9625562cacb7fc" },
                { "sat", "9c900a5a3871d7bcaf4afc19830872c4941f5ef5faff36b60d9b4afb440c28100a208e29cc56f2f47e61ffa97cb7b9311251363cad1f153f7e426b2570aa1f9f" },
                { "sc", "d75ead3f2f10fa112893c8c6fcb52dbc2fc5070da4f18d9112a3782de8a445a236efa57742739390a64b71cb21cb048cd88f27e70ee9cf6ce24a2bcad522d0fa" },
                { "sco", "51a8d97ce668448b2b090e5c018b926a4a98917ee9537200545884a606f10547db4283335a0feca316446248065bf2f1976d48495f58215601caffb2b7d3971e" },
                { "si", "1e7cfec264ad69681aeab7fdca3c083662ff8fe570ad8bf43cb050dd086b42003824868159d4edaf6f7a89dbd6f7e1fe5533d2f90126cfeeed5acb5d46bda85d" },
                { "sk", "d354249c038e4d9bda8087adf481b34b193242e8c137713f730742d082420b80722e9052da3bf9270590912d5a304b1ca4a3358d348fcff96f33935f54250a0f" },
                { "sl", "c5d3c9060269f90ae829dc16a292e4176a980e74ecfa2c874e268f13b73c8efb04d9a0eabf8897de5577c62ec1465eaa2b7c0b008f62275fcdf6b47cf5fd9f42" },
                { "son", "9c618838b572ca77f5827309653e3b4014cd6e9e83e48d49d1502664cfcb422bca226e9e35e61897b8cd3e7b77e7a45e833b4fe968cd807124c2a91ab904cd94" },
                { "sq", "234b93df1f3e930766af02e357bddf8a45b2ec3dff39041a9b5c9209995e255b6ea35208f9436b21dd5550a0f9f6742627620ff59747d340962efc40db7073d1" },
                { "sr", "65985272a279f8b0d70ff0a4bf1fd22f1342de5bbd9f820d8d358cb7b14a522b7b21ca220b895d3eaaabe949a3749e9936c0f29097d7ed79522d8593be3d69e4" },
                { "sv-SE", "cdc9f08f82876100a64a86024c553718db17a7674d2009b99727eaa39a843afb3c3a0e9f306d42ffd35f96bb6dad8c900461044307fce74a85a09d12d644994b" },
                { "szl", "78e194ab85e83d6563f25ec0dc81c9c431ca2b049fc1d3f66c805c4d4afa838e6e457017b66ecaf7af2e3646a0d11935944578300ae92544626357e40a35eb47" },
                { "ta", "8c25cfa111b7988d93ff11db5699d0b361d427006d7e4e33d1ffe4408dc35390a922986c07db751bc720dbfa06ec7d535c34fb55ea78eb2d918edf60dccf37a6" },
                { "te", "36c07cd810503d275af74f43c107f5eb904f98ec0e1a7e140efd1650f625be13c95b7a3d5e604ee63464240c003712ed47ff840ed24c812aef106ee144cf32c0" },
                { "tg", "f3f078fc56a19b8bc9ddb775851a70c310bae2d4916dae05599b79511cac16a7248ca727a5a5abe69755d5b6c5c27042a2442bfa1d85c41ae7e2e2ff456f7c9e" },
                { "th", "9f9cd53ee6fa03ac65c5120985732b510aba73004716fa0607078a94b447ac6cf8760dba66a2f7fb1792c418889bb197835da130dd0297613664fc19679619ea" },
                { "tl", "f2d3251d2b1a6b2696372a371e47ba16ad3098b8caac83f65a52294c78ff861a5b0c4c0a88847a8edd74a89d9cf38cd210155657fbd560de6549d8215e8777ef" },
                { "tr", "1796ae75dd09d8b15c9f0629a1bd2cb06b1783cfaecefae708783f506036d04a4efa9c4f71bf5cc75b5b7eef6476d646df7485db0471e4e6ac2b64a18e2cb6c6" },
                { "trs", "d320eb6d0af9fb3b5856e53fbeff9549e52f6f14e145847bd2600dbf1263ba6f2668979df213e7929ab5d526c1c8326bacb74b5fd9367d4213e2bd4a682648d0" },
                { "uk", "cccda19e4c78d22329f733923a4fe90a990e88ac075949cacd40d10581390acbb7bac634cccb390be503d8aad8761ef8bf8152dc3b93a4f424bd9aaf328daf66" },
                { "ur", "7a87f8213d0d5f669bdfdc14d757714393eb645c175d78f08e14f595a44816f60cbef1048580565d1521193dd6497bb96c4f7bebab3e6baa162984fc305799b7" },
                { "uz", "3004f72675595942f924c35d63bf22783dedeef0aebdb4b0aecb5228c31ba58e16c7a45ca34aac32500eb7247ff42870eb925b4f1a51ecd574f4278323b7c302" },
                { "vi", "77996a215685dcfd3d98306c8a456345e251b0ef781c9072ae808ee3382da9e64205ec8127a56ea82d3e71559d1c25c48ac50e71472cb4e63172246ebddccfb2" },
                { "xh", "70aac979de10c359af12fec9526ca2ff7be4f89e8022e3c093bc63bad14aba47b22182530636ad6567c22ba18f368d8b7ff1ebf3f7e3eae78866ea1dcf242302" },
                { "zh-CN", "064c02a1b2ed9caa066ee01b9dfa488fb210d484459848e0b22f9c8d664d762f164d0b2d894cb443603b35c9806b987b60287869cc042197c0eef4ee60d9ba0f" },
                { "zh-TW", "f3a4317b7a32c997fbf80d61e35f2c32d54f0d46e2dcc737f968a04fb8c5b105d0ccfbd1be9a11c1e6fb8521ad0bcbb15b961b6ebcee88ed5c47e4722a98f4ec" }
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
