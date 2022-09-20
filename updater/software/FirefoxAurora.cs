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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "106.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "52b5424f4bf230eb1da80dd80eadfd7b232b1b69c3e2c5b28039b8bc9b287e176b36499792c3a56024d866cf3ca9823147969f00cca50c293bcde166b5a4cabf" },
                { "af", "6152ec4a4ff96935d98d80ec3ad81dd61affbae7fd93bbd25bca389c7f3d1e67d0e4284ae719ad737e241c3f7a3252eaa8da1680587e39466991d20e0c90c89d" },
                { "an", "d492fdc9dcb918390d34b8714dbb7280a111f2da1f6b39835d417d00674dc6f299dc672ab2ef9af69df3533d5c171f0b6f9b1034fddb25e1b557847abe72f980" },
                { "ar", "d2c5561029be32bf77ff5eafa4c6baafe240db2ec3d168bc783c34055e85e433799a7c6f2aa25f36d6f39e578be4a07ab2b0294515223ec9fcd8417ea14c1edb" },
                { "ast", "9b5c69fef4038da13c06f0b3b31446234eaa45c75008ea112bd649e447d99484277df5424d320557d8e01aa73e20babfaef1bc7e93d6d8e199204c818e8e5443" },
                { "az", "fb958953054f249e84aeae5f889e43248d697a2a1f2201167481f2c4e5289839943fc4e9dd016ac0e0d530077152a122e1ab6d0db81f9d61238f005cd25c9cc3" },
                { "be", "ca43cc8a2df588afa1886ac9f365814d2fbec6cb7f07614229ee0be63b9a396e9687bade2b57b393edc8dc1cecefa2bde3c2e443825ba6d44de1e67430402581" },
                { "bg", "e7fc0e4b4294badc68236610947cae2dd1ed526c5b624e922b20ed7e5b8396e952df9322260cdde5c72dbeaa5f67f8c5dbbff40941f8768ae395132e9544776e" },
                { "bn", "ed8b803a1274da00034f26394b365bcb2ce2e951e4abe229518d5b043be774e78b298a824a3b0196801c6da4aa039c762fe2c9b1f6e773a4ee05a574143dc61f" },
                { "br", "906fb5a1b379f1184ee502526bf5b9a7cec6ce648aafb806ff6cdc626c0397c59696db73924ca44711639003f551fe9244d468e3c956cfe0c7c16453fe83e950" },
                { "bs", "d03bafffd7171865850024b98d1e3b8dbfceee21da2dd2a3da006ff819df1a9178c2535b939e3f0a7a53ddbc228777d62b9d06e6bee348a40fcb3fa838c5a649" },
                { "ca", "734e1c5677eeccb00a708d4489a5c603fe79282f345fbb6f449bcdee444823584423bf3f40e640a39bebb3bc79d865be701e42801d9ed45c59f2c62c68e4aaf5" },
                { "cak", "df6cd68c1d7ac822c99a88adc591c26490340298098406e2b116bd2ab12cefd71bee9a312b8836b87a3b39f3f222e10fff4b06161df945744e1e491cfa8c079b" },
                { "cs", "ba037487cfa237d73cbdf8a347c0f6aafbe643a9112c55040cd7c6573955ddd9f8eef7058118666c164ca10d891e160019edc70cc60fc4837fac5b3ba28a893b" },
                { "cy", "4a6002347423bb05a6d5bd67bf11cad0fcf3058c7f64290c6f4157cf0df15ba03f5396fbc93fe853dbccc01893c44f2c4c08c458255aefb795cd42e5b4c6275c" },
                { "da", "3b1e05269ba2ea0a762278707d2c9167eb0b07f208bea251c1e802dc104e40d9a28aed9daeef9290160492cdf3de8664aee397fc4e087b59f558f817725f11fc" },
                { "de", "9853caedd08cc3cd1776c928cb94c49d445231889f67cf63f92f80aa7f055f6700014cca34f04ecefe2ccb757e3fb6e98257095bb7aaafea87eb85c4d5a7441f" },
                { "dsb", "3c5c14bc0bbee89fa81cdd8baa063007a31f8bd8da115ab63602fd4915b806408998413a4c0dc170f56170293a621eb054c88f7703669ab6e718e87e3b1e22ba" },
                { "el", "3478752028264535147f8ef05406d48572a6e336352117d935720674c770246b00e67bf2fbfeeefd4af402f5da9be026c030a2f71476d0ad4ff5ee8242c5dde1" },
                { "en-CA", "b20386f48efd9935cb9aab993b79ba21e4546ca5b0ea3725859006676be7199b124bb25708adf16c12afc3615c6ed209d7ef02c1d3b998a698013ab4d6309203" },
                { "en-GB", "604e15211e9e08f9ffe2b813abd9a03e728eeee7dd28aca3d70ac92e2c262d4bf22cce08ca081a06d63ccf6d9d6ae0d8f7d5dde267e850b1185998878fb7a773" },
                { "en-US", "4abd64573f63b26de529cff69c94d09596aaa2ae6429ac0c5d14ac9ec3d6c1b15e94296015079de59379ee2cb761114b6e4ae2d60a54b2e9cf57568cd3e15d78" },
                { "eo", "cc09edfdd690ad832cd0ade1eeb8dcbd4e03410bebac61cf5dc16a63150d7905ace6da6b476b41f972fe931247e3a79afaea985554fc9c470d8596ac8e4e91f1" },
                { "es-AR", "d999f314e1d9daa7869970bca977aee18ed124e27a58f58ed2d3a1222bee9d2ae382254dd4197623e04330d9e5fbdca34cf55c4525b5207525fbea08de0812f0" },
                { "es-CL", "b1e74245c68942e610fcceecadf292fda81eacf692baa1ffd79144f24cd8c4a76b7f65def94877200d177c8cb71fe047c2368a84b147a1506bbcdbb9c6ee2c5e" },
                { "es-ES", "a6cff6aa59225c245ef008fe146453271f88f83251120aa0e34fe8dd0fb56ae4e2d49ad225dff014aadb260d88687a639d0747ce1e4bf960850434556322c732" },
                { "es-MX", "1e2959a635a8683d92fab4393ebf02ee401b9d88c8232055f7908661a4f8cabcd172786f47f950721ddd27ee4aa898b38b05f5df70aec492f6cd15e8a53433ef" },
                { "et", "0cab1644847953641666e8974f2fef4d652e2fce0ef5a3ed387c3eb01651ab25007e6fd2eb409737a48d8bf828c43a053ca4c1e50860a295c222d3c085c2dec4" },
                { "eu", "53ffa62877e260669763689efd513e23b63af116ced8b8c7a846cdaf81feb5a42781d89eed599e721f2d06c5eb9a683e18f598967f4cc9dfed704813394178a0" },
                { "fa", "8be414d7dda8e51d55490882d5afe3ced83bf9b74d76915df37bbf99833aa20b4097f4e99d8920436e108c6c08d9e2c6974d368accf8523241958b48b4dfde17" },
                { "ff", "fe3aef20ceab7ffec2a4eda36ccdbb36440b44931ddc13780cab39fdc75bcf00852dff6b7b890b50052569163e5ad8ed1e7c260f02e31cf912568bc94221ede6" },
                { "fi", "2366900e23f752f3412eb037464659efc88f896f75c3d25361cee37c330cf0a2e04e2e719366162f568d8e98782fbf146427219c014976c51f72b636ed8a5d84" },
                { "fr", "5078dbdfb44cbbb0d8f36d172d869ad4d2755d91b93430805619a5d5de13c3f9c51b3077d35ca7baf1b7f6cbd4042b76ea390e5cc1af2f4ff39d9401c0e007a3" },
                { "fy-NL", "7bb4ad8f5474df498bf30fbf67db025ee9ae5f43a61c057e9d26adb4037561afd5dd4d030700e71bd7b517190cba38c8321c26ae50f7b84dd7b076ade3251505" },
                { "ga-IE", "195576c939a5646655d8ebfef6995092cb5ca31588fd82b567aa430884857beb6a296886abe8e780367b5d118b72b0f6ce9045ff6bd6c6be8c36d3cf759390ba" },
                { "gd", "9faedef01a4283bd039b6c59841c874433443e480a31fe154d9874a90b2efd1b1a815311a0674818b5f89eab0425d69ee9b94b55c7d4b69251463675a254e948" },
                { "gl", "1fde2236d46c92673945e0e646760b7ea4fe65cca2e96b105b154508d45477df0365c9c14ae89da445223d006320ad7151355a25cccd6026309c47e19f3a7382" },
                { "gn", "5aa3ce87d50479c5c9f18f3e7d338249206637543403a8f0be99ddf1b33956aa7ffdb3116894b6e396f20dd1bb4952447b2e6124e01c72ee1411f7aea93445d7" },
                { "gu-IN", "fd5923363e526d4cb077a1a9cd425fbb04f8165bd4b40897d812b22605f364cc29317286c0d3ebe378d1267541b2243f3ae0a0ebf332b0bb605f54e9c9a1015a" },
                { "he", "55bcd7dab784268240d0b5be56b289246a5c3249f24601234cbdd7444d78ad81b3b470052e0a6b7cc87374c7f120aa982ddbc00790599eaf77593daeba75715c" },
                { "hi-IN", "a0d6a445463ffe075b72a1e6b5ff445028ad7ea39d8e7a03d92127a5f3873977c1680ba9fdba4c9d70a2dea02674f274b8c3ac580e54209db29bdc753f290f0e" },
                { "hr", "e9c855cdd9503bc3f0a56758ca37ff30e2885ae092ffc8861ae67121728c3bf11562847b1f38b1cb193b6637d6bcdfe2c98a480d9ae56e3fb66cfb1558b8931f" },
                { "hsb", "6137728e276d61c1575ac123279ecb22002023481975020bdceaf6957f5ff3dddca5f7ac23ec665b11ccd5aeb77506b42406fcf4f856d3f562b3cb28d868b504" },
                { "hu", "afe7d953fe473bc1ad6bccc4fad426b4913d44c655f3f34d72ad86540bf5559886a486032b9a152b00f0fc6ebaf589eeef270eda53c59fb21b91d3694c0858f4" },
                { "hy-AM", "d9e42a734e5758ae22786786ad88a67a35b8811edd4fd4c16c502a9945454d1c68f7b3158c311f72c2248355d27fa12c3cd266a8e31c4ca4620d8f3addc41df5" },
                { "ia", "6a87f3fe9fa6fbf6a8126c7d4cf6284d65d5edee80cb3d994ac467f037b57108e58f7b58dc435df1eeebce21d77eec34b8a2285d5883a98dbb693305694edb76" },
                { "id", "017eccb819517022fc2de6a35e29caae66e6cfc657c69f9bfd777751948c5821fde116b7604da8c68d3607c9228aa133d68a9f8bc02fe7a6cdc05b2346843861" },
                { "is", "6b4210d2c605f9f459accb2894fd078f6eb375805eeeea98e975589ff1ac0a4d7bf85c11348347011e9a26ab58b69b30ed44b4e250e33b50b49490715e87f6f2" },
                { "it", "d904ffc75e02efd2d300ba476e92b5f9493755d35077b57389cd437f41b4c5dfed47f3270875233c62727ff42f8d1a46938a6a89654d38663b91d1a31d725b34" },
                { "ja", "f681f2986ef783a1efa1e0a5b3a8cc7d4b54b6202cdcb7d3abc8dc0b0f70b3c106adc9fae75fb6271a7da9a268904b94396554d1ab2ed76b45e37d9fcabf4ff8" },
                { "ka", "8311e89e270a9a01065726d64451deb6c276414dce9ea2de6bb00ac6187354b59e4925c00220271412a0512fd37c13475fa30e6fd7fa823216c02cc2db2e6336" },
                { "kab", "7b03c7ed302d60a1f69fca3edf6738ef2e4a7c5211ab7cbf98a2bdf3e39c011126004265a2f91d5ba0f48253e11135a131f2f6334e165bf82c5a2481cbcb36d0" },
                { "kk", "193a8e8569f47b4f6929fc51c4c1cb94fccc3d7223d2e487be3f10dd286d4832c01d1773b10c9524b3745489328f09ada53aa53a97de3154c4fc1f058b4d1f1a" },
                { "km", "1da16b506544c146c445148d9284062fab700cf15402eb195c80b7f0df38544bab5f00a587bd83726496a317386efde8bc63fa25dd89025005cd4f52b888834e" },
                { "kn", "bdac5d269093613065c5e9b961dad02560f5f8854dc5a44f6286ca2bbf08ad4f4540b2c43586b8bc60bf0b1fb08192391fd24e13875d554943821c1660530a22" },
                { "ko", "4084c021cce5ac377184d72735e7dd2aba88fad1f63067e7757a760ab728c01fa1e0d6a25a2e628fca9467c8afae3f888089bb8719663696681592c1a8c64df6" },
                { "lij", "3a616e13cdb456454c28743bf374c692c92140c6e719bec3e6e54803eea9278edaa44a3b27c976691793b317031871657adda06d0df163a3382305f81bd0e700" },
                { "lt", "dd9cc344ef327f386f2ba4e3d678982635312f71977cc15576acd821f2d3974d0ba2b4439657fd68d61ffed55ec44a40834c416d5186d8dffa23fa51d0fefe73" },
                { "lv", "8e7210ad78f6e91daf7c51a2210ef9f2314f62081081daf14b07f8ea254106e190f538cfb3d3650b678647315e2ba1da9d829aabb7f35351201ac6fd82b83e4e" },
                { "mk", "3238115d0607dae62c5e45c3b4fb5086eb5d8a862f5d02c9785938263a2f3cbdfd16346e505628c9f755fcb367d428c3963a1d345457f98a4b523e34fdabf356" },
                { "mr", "bc407e4139266bc4dc5a200cad44c02e7fdbee650d55801c8cfc12dce5f91545d1f7f84fd449832b3435a22b74cc4ebf016762af6c23c355a85a230873a7837f" },
                { "ms", "54974a8cb91b2092b3820c60223c06294ded5f15d78f89476549edded574e22ab4454c0c8f3381754c1cc306ee4421ea9f72119915fd1a36b33f14089986408c" },
                { "my", "54465475324ff4efa9197acdbb7656af4affb1eef62214c0a67abb22f67beadb7e72a99fc5a0d8c95496b1eebf8b14c1cacd465f04be0820673670fffb127bab" },
                { "nb-NO", "fea90960b9e0fe988f2134fd0e1fa8351acede7854bc84b990379fdeaea57f76dea9da973e7a89634993f6c50f55a4d37d80843be4c48494d7eda8eff17b363d" },
                { "ne-NP", "b283c3f1f6f375decf0742b41c76d407f299fd316c79d6b92332150cdeeb6632a04cd72cb5a52eee2b171b3155902ceb9ed75f6bc646d623de2888a2ae8e62e1" },
                { "nl", "b2dca856732815265408ca7be7fb8bc08a2676b3f216301cec9976cf33d5bcbf95c21e6251533c4f74f5fcb16b5ec378dd55a868f17971518709460c990e9d04" },
                { "nn-NO", "b063093c87a9c9fcc40a9535b078e3d47ef980ea09462d1ac3aba83b57bd9f1f9e11aa1a3079cfc05687da306752f18ce849884eb6ebfeb2c820d5ea8289e45c" },
                { "oc", "bc3ddf425fd9519e7a87871c0ff9706ff7593c43703420c63f59600af7616bd63d062c9a0b7b64052e2f70bf35fae607028b41c725d0dd18906f01404f58800d" },
                { "pa-IN", "572e955fd9fb12332c8ce83acf5b79b8bb6850ee33b63a467beee3fa0d79cd0616a57a4655dad6c73b103fc87edff9076c506fef80093847c980032ffe437428" },
                { "pl", "d32c01ef3d424d5915bb73a3be2968ef5d2f2b798d53f48f496ef963109e2af77b6a63c18787abe8b358fb5fc2b4d36666979daea180427e2fb4172c86fb548c" },
                { "pt-BR", "6523359ab0b967c71c32a725d682dc8c68482f978124ad30009191eb6bbc82f7124c102baea5fe1234805ac0615df6990cc87e321cff65f7b500be0c66b994b5" },
                { "pt-PT", "d3eb5dbde44db8cbb6d2ddf2bdef46ab6f6ac9804e1e659e5ac833b8d2109fcc53c29e82e1a1141f353679c4ce9ce9d40150e8a6627fd8e3699e824140ff9127" },
                { "rm", "3389774429d08ef04d2d40e094a752391d039115ce067325d98e62ab3610279a31dcf14eeedacf6751549cf7d05ec7f68f2b01b6597dd92fe94fd5874500b1f9" },
                { "ro", "90cbdf85f75f5e06a9ffe17d0cf5c7e50a29d57e2bb4f7061a5962dd20271a0ece63ce46956309c723f900ae291ece050ecb584f5ad06ce1220bc05533da5242" },
                { "ru", "624900d7cf15c75635ff806b045ebd456728c1495c2a86dd17e24e05207d11ca56c7644bd9c924442d63a55ad05d66e91bebaead98f5a7266e6eb2dc73df7e97" },
                { "sco", "32ef1b81d952c32f81ad6c00367f65ab289782bc370f78dcf053eac7c823bdaea3508e9b16eadbf30f2172d9cd8ba3d0da1420ade8cec1d945e6ff241a1c5161" },
                { "si", "586b4c34f84a82a0d171a53ccffd9cf75f0e736d30a789cf2c4b5cb6891f44de622a188b3ec0dfb3885947c1b30edf5833f07036e5bcd46bca5e264d5bc4ba89" },
                { "sk", "2e05cc2f07e38a1dbc6a74004e76f09e1b6f056640d665e0fea72fe48331458e4e1cbb74be6ed8cfabad5bcb00125c972f9b423fe6721d86926e2f891e6884f7" },
                { "sl", "8cbd683be179b145cd6fb6b60efda12ada61f8fdc639fc6998e39ae67b72be1e621123f96ae712104e92bfe94d9ed7090221bf85b7822c142e0df329d5986f68" },
                { "son", "dac01bcd2e34ed1cc2e3d8938584b8e994e595a313a30178659aff1ba10aaa8ce3aff9736b515031029b4b8a533b84cc0fce6b7bb26daace67b6bd04ab5bc2e5" },
                { "sq", "acf756d43c2c5573bc50b1e1c105da51df5a78120317b0652eba0b9ebc9956fc67112750ec5098413debf638ccd49686379aa0642542916f5f5c8ef6fc4a0706" },
                { "sr", "5bd50d405923b89290deb0dda5149c451d927f3e58e303fd0a6794f8269b7d0e952274b2349a860913de73490a702c20c8a9b00ded5258b9ab772fdb1965e7e5" },
                { "sv-SE", "fabe4a5231f007e9fb8df2b47135e77ea1351c13fd663025af30adf8a124a147fb62e99d8e8f2b8f416160bc23af29f09253e4a4e9c277dd24d7030d9caebfed" },
                { "szl", "014d52d2b645fda372d8c479551d356f960ea135090878640fca1b20e72d825477c2795950f49698a36c1457935213a01bae4f967b1799d10546c735c83c6c89" },
                { "ta", "3cb20047cdf61e5d133d9643fa827616a5663d5f0203a93e292a3dbb9f0cc8541d4e085df4ca90f7c79a3395d77fba98136263d5a7128ea8054dfa0828aee015" },
                { "te", "0f334c467380420f422b1176cafbd854c4a514252b2632d8dd701b3733fd0ed4eb314b613d6624a9db97f65644d66645df54dc748f16fa67fbcb9567afc73dcb" },
                { "th", "f81698f757cefd919ff29bc053696b5bd62fb9f3cd9509b3006d695331086baafebfab9cee291ed46f50367cd69e58e270e6fe359583bd2013f4aebbc2483fd0" },
                { "tl", "2ed39dcb32cd84c7a7a2c6fb97e8ba3913a0152867818fe0863e86364252e26d32af6962ffe200af646d1bfb40d8bbd581e0857e227aecff1ba039ff8e4f8d40" },
                { "tr", "683e36b9de9e8306d21e8b77147eb0bb0f0ec47b3d197ae02c2f1a8716b402ca320494e09487fc4d3f2540a0c4d4163cfc51d2d97bef09b7d9a7152ca996848b" },
                { "trs", "9151d6a9e62c002253810f8fee0ffa8de0ee844bcfc20b02cc2fd4d18bb39f1a941f7a51679b90a52c4217d8a9ec7dac0c5f19a7d118f453be0cc3cae0dae057" },
                { "uk", "32e4d22d61ff13d3810d2c1ca3e5f333d8422a855052cefbb26be04d8a4b7c1b5bd96b4e05b0f387e997e6632eae9283d596280f169567657074ee698f70a134" },
                { "ur", "0179854a32cfcf154edce046e9f8f4fed8792b1c8efa11a55f117204c62d9c2d6da864bf65a6630425e2da946628fe6dceca8bb51af4fe22e14e50b109dc29dc" },
                { "uz", "57875b6d845cf169936037acb56adf1791887cbc4f9dd86f093862bd55227b49db2915471e02b466c04807890a5e9c34ff2aba7e737bc750e7eb1bac69a76788" },
                { "vi", "aa29b6c799b8c547e6f1a7a06f59fac028d28ab2fac62018e74dc0d7fe537d80d00a280fe8f242740929ff10a9fb449212158b720729c4d7ef8b7707ffbee27e" },
                { "xh", "74cc7d3bbc01d7a5150e49893e50cb09f726355d0946eb1c98cbde5431d66ca8d20d342db4bcb378f8658ffd75c785f34d8265e05900006938761f9c9bd7961d" },
                { "zh-CN", "fb8a889d34e60dcecc5807994eba170ce45074adc38e508fe6aa0fd864065054bf69bd09ccfdb2df9d04b5a881f7ea499bc119bb1465238bc908d1dfcedbeb90" },
                { "zh-TW", "651eea961ee983fbfa759bfe786a99bfae0989b03bb9ded6ee130dec363a93ab5355bad309672878fed878b2aa979973d6fc4caa696c81debf5e605155ae2a5d" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "675ea4276c1a6af8f31cd4b35aba43986ee259a6acf72057bf4501a4a176607c4fc020a3772fe51492776e873c6edef92d559367288282780cec6794859abb6a" },
                { "af", "bb2c1f792adab6e411f7b5fdb30e668fe64b8054d30c0d1ad1fb096ea03ce3025811ffad64ad6a8d3b6cecef3cdbbdb0f59dbec021d1a43454129a1e94d6f257" },
                { "an", "27862c32112c5533c69eaf2cc9e52de9309d0b6322214bcdeb051fccc1e5b5c0ce900b71eb4923a319f4e2a17e27b7f89c0ab4facb4a8cd1038c4917c750bb98" },
                { "ar", "23267d06e681c3b4faf2a48d3ec5557b4fa05875d8301d1c369371312a94b875cdc06b43f461198e46ecc83f4502907bc158c040133a08d453fdde9eb4cfc348" },
                { "ast", "e767f893fe82dac5a7d7e61022f5e619241eb775a7ddebd04f766b702d2b7f18bbebbe21270ced126da5ec4edcf79aa77d5c13f45dea3d4c48ace549f695a058" },
                { "az", "59a2416b12922efd0e1a70c29c44a014084386ec95fc954f7cc69deb87bd3768141e84b96135c19b2c4be39bbda8fafa3234d3ab06ef2c01363bba4869b81ce1" },
                { "be", "655d096bd0d23798f2d613e52ba2e1dca402e2c2da4984689c340102f020aaafe4023857f4fc88f59316595d7cb4d92b3ea946dfe8c46a84a4cf8304871dea0c" },
                { "bg", "a8aa5c94d3f4cc79125f6675f89fd942d6583401f7fda4b98021d9f68c8a69a1f0f049e6f0c73edad5df51716a27ef2ddb7498a7b5abcb21135b6eb24a25b75d" },
                { "bn", "3bbad8ac5affb425089d559a0663f7bdf3fba481ddbf6ee7a867336c705a6dc5c8eea17deef56491ab87ec697c78cfd1648c823b152cc499424591e248fa6d21" },
                { "br", "c61d198a572a73ed8eaac3c6991d6edcf97e82b01b7882d5659d44a18ae5f895896262093aeb5a0557fb9e9136365903e6002820639ec2e68f01a3212f3f2aab" },
                { "bs", "755b7ac14d3c206ee6423cde7cd5053e8e4bf417d2359516a8ac0ee4566829b428be69bbf776f50b80fd50a81187992e61cd49701ba8d6d53e45cfec604e72d0" },
                { "ca", "0843df6af9408ce22029f699c005d03bf14a4adb0b04e0da7eae028b712566ca40781496e4979903b9e27ab925aed787a56314319788f1e1aba283590ba66f35" },
                { "cak", "95b08306b3f3d7c5290cd7afac2b844974ddcca94c96cb3635c0ad10233f4299b517bd171ee59b2bfe2d8f149415ab262f466db332986dbfbc65c9cb061eedd8" },
                { "cs", "67dcff8ba92901b14100a18c33bf01520c6bf517e06698d387e78e7ff115fcd7cf8bcb590a101dceca3b5640c419749b25fff20f6d4797e4394d812f2ece8ebd" },
                { "cy", "afa37288f417547da5aa6f3d28a92208c83feff999e1c8a44531ef02715bc85c5c7c1fb6fa86fe6117c707035733e56e5d22eca5139df23728cab9e3a0fccf5e" },
                { "da", "477e44d42f2b48887c187e5f0ba8263e53349ab15ee37e5133b4477e7a5fc1ddfdff21ce454a903797783c1949f9ebe7c44a1e4ee7f177d6459d319d605ad66e" },
                { "de", "ebc92f1d1405842e6c32f69b848037c209d20a3b8c216517383066ff2463781b02dc9ee75a3725b9f1eec0283961b0e36b4123b339243b5d6ca8cabad946f3c5" },
                { "dsb", "276e425a42ba688eabf9bb2e6de71e80eccae3f978d672a6a59c9b1615b2c5e4737556ac3c8c6fa22a48463a6699781ba2828118da1dabffcbeafb51cec0616e" },
                { "el", "29297ad7a6447e29409b37634905c19f7ebaa518b9ac2744fe31e5770e073e4f3f1777f571ebd781650e8e1ed7e37c9aa85146784d5f5d33e3fbca2d9e4e5d58" },
                { "en-CA", "7bfd3981b38d887d71fb1ffac363bb006cecf9f040d561ff0bda221c0641b8daa145cbf472e57af8f1b81572b8de62a61a3b460990f99859af3685eed2dd2ca3" },
                { "en-GB", "3e95b216ed80c0b571158bfe1f77400e0604359b264c3427f56b3ddfec735cb928b3ab7b6a1198242264aec69e4474cc6bafe0f31b658e2ff12ebbb73b42185c" },
                { "en-US", "9f6046773653d5ef07e58090dcf1e92550d93d45ed14be82f341216e01b5306a42530f4fdc13907e1c1c3504d3caf2615007a27484f61476741e338ac6824ee5" },
                { "eo", "20133d85a234ca89ece575286064c24eebb75f9b58d6bf5ea4fc43d9467dd345903f78ddff34e93a136700a15d68f2c143c4a43837f62b7b18364a0f60fc0aa9" },
                { "es-AR", "2cf7e4835f9cbf4e40c126ea9aad2818bd0f400569f28632dde841394fdbe2fa51b483c011c20116527b7270cbda3969150f39c83287efaf692205cff408f6da" },
                { "es-CL", "968dd227cbec33056b57a11fd3d758236713e2c2b85f2148885717d6da43350120551899f8328a83c2f8435c6f48a39e15dd17315a46e12791346631b623b367" },
                { "es-ES", "bbf1655873a35460e824b27bafe2996abe00665719682b13f5908b4858b5d98413c1ff3c550de5ceaed19e526b4a2cecfa16c79d5c60d6d9a33f559a9d60fbca" },
                { "es-MX", "c972c45cc3e5428556641d8a006ced74a59fb324a5e8db08799cc434d9c83902fd6f4b1bf241d275c4b76427eb65abd55cd60477456e7fcb7713d51ef953f51b" },
                { "et", "26f13e94e35af52a3c23f2dc14da92209839c40f12f95ce51a210dd2d5f5d20b3cd85f60a57457b85ec7a30707c479bd2d755f67f541a09e7bacf66274c06fc7" },
                { "eu", "ceed333806224c521b8683a2e9f298d6acde871d82ada2530fec78b838030be28427c952e9a0dd25966dce25e6c095b15fe18cbd01be24b6ab2a15b1c940a2b9" },
                { "fa", "c8b7224b189907bc3c1e6aa7e6731df9e29bbfe564c65421d01ee04d8c5abefd0434058e55882b99d155297c84f0a946499753bf9035975ca8cb6cb9f51e2cdd" },
                { "ff", "0f29086b89c9db36cabb3bf3be53f2e96b4df965a75e617f2bf17f1699907877c92e08bad8f09fd897556f52956ab7c7ce180d8eb6bbcb0cb32ae64a7c32cbcc" },
                { "fi", "838d1c112b0a5707837c2ffa554568ddb4435002aecad60ae25cf17729bd989cb5be3a39c72446f797f6d6674b10f6134b93aff3649b131ac29462b64e6e251f" },
                { "fr", "3aca7e4163392bfae02eb5032c8ef5458571d89bcbf161b6a7aad736c197759071c93fa7eacca3e3c13968ea833e5fd7045d04a53a00f2a08174c00ebc779cfc" },
                { "fy-NL", "a1dc84e17fbfa4996dfdbfe261eebb17cecdbf97e266f02aecb4d8008c865e0d2aaf02b43ca2b947f9344eba20959a683ff0db49ad34abd4270cd3113751db23" },
                { "ga-IE", "38650622d8976c0ebfed98c0ff3460124671c8c037020104653179ed359d07bbc3e000042930313549b479ce20ebae14a9f26a5973864d5e7a7e23befb15cff2" },
                { "gd", "1e3025faf3a860936c23be9ceae2d9a225e8d95d9ad4cda6ead2cdd8abd39210484518dae2ac5b20a258a5c7c1dae75a8a9f68143590d6190cbf469cf4f97aeb" },
                { "gl", "f5f8ca5df7d54355a509cb290dd3cc83efe44a3830e5e0c7ce875453f5e081b7d2c039f251b63bcf3feb911b07e755f30bc5e2bbfb4beb6d77f89387aeebef56" },
                { "gn", "6df01e7171b43539f1360dea89304d95a8a0251c19d3c846649e9a13f4306a607d194e5695c5ac987bc002a33f15145dce1eca7a72c95eec5cf3658561de0fc7" },
                { "gu-IN", "512c487751c022d4a06ff71be16897760415e3be271c0d4d74c32d852aa1840306253739f3ae54c579554d28af0de212fe131123422d1269e5d9dc41095d58e9" },
                { "he", "f32690f0f59ae70197fce80d05d936639928de7d4c054aca179a1636af18fd3540ce43f8d93244a02319883791fa01006006bb5d69d499c47b7187fd102ae98b" },
                { "hi-IN", "0aa1a43b1466864cfb58a85870dfea0fd561701d8fb1941e9f5775fc28470ebcbab1f5379dda4564830b5d4c4839507dfd10a61242a206ed411decefba38d529" },
                { "hr", "62445e443973a858803eed84a6cae0a8ffc411625b7cc5928f7b92e882feb47e28ccc41dc54c2063b2d7ce5a45ee683cc23807178174a928ecf30353bcb89efd" },
                { "hsb", "a62ec31c297fc1e4c750853d547b3d7159e94333f62892e0a20133f7960e5d2933e7d8ac97ed19d16381963ce9f5b07915c8bfa2c5ac4d0533b1e28ef10430b8" },
                { "hu", "470bcdbb735146401038ba852cae9ada28825e677ad95f9439d03df6489800032f957e15b0fec8a2096ec9406d1de24994f9273c1a82526d36bacdfed2c8e371" },
                { "hy-AM", "73973f7cb548535c6399624a4e65b655c91598afea06625ed34cb066acaa2bead714b9b5f8c0ec229f3b4ac1ddec86c1f2074765330c60c85dcaf942878ea38e" },
                { "ia", "2b9a58d7ebc19f195df527b50ec3057518c8250deec707f4066fe47e97c4fbf23bc57b8088d1c1aebc0de4a84f0bc2da23810f1f1ffdcb1b25073ecb57a9e7cb" },
                { "id", "ba37eda77abfc15f26ddba1758cff00498ff84ca4ce815dc23927ce33926894720c3367aac1343e8e601ec454452da02d603dc0a770d177c1887407f41fc7ed0" },
                { "is", "024205e068566eee86ee74228256a91df75c9942c0650ebaa99d209f3b0b968cf4ccf43addb14d6a0a8dc9e7945aa30d0d1c1c9895fade296d7e2dfda0df6349" },
                { "it", "4e20036bbf7368c92f988c7a29da6ac6e4bdf2748a37fb33f1ba807e8319cb773ce360cec86a7a9c348eebc3c9d5c2a60fa7dedbc9414a4765edd198969ac399" },
                { "ja", "98045fa93d018aced5799ed374982f203016196fb7c1cad41b0b99f12d5285c3d3abb8be466d8082285e6f00b1f0955b9ea0048f6795776d20a1166737bb7192" },
                { "ka", "bda8f9783482ec03c70c62f972518f42a4087edfda21872c98db6d94375640b74eb244cabcd6bb5799abe93078ccbe57f661897b1b595129d83f66af3b7a0fce" },
                { "kab", "9ecd8dd27a8fd74dc0f443a0ef92d96efb1610e47ba29633de825e6d5197dd9a99cca4ce8f882e8ec9e495f9dcec4f9356776bf2a391c253cfd71e8f63f3e06a" },
                { "kk", "086054db85dceccca1fe843174bc913ff66cdc850a465ee1745b39087443272aa7664e7b2fc666c641f715261cdb513049f7ca64425cc7df7ada1a9daab5cdeb" },
                { "km", "f916fe08dca7693940eecdd4585db12d58e14f993c43fb8ca49e05d39bec39bd2357201992d64c6805afad1b24a1a99596bcda28f00f55914b22011c48c3e3b8" },
                { "kn", "b346ce68026ce8e418850b67991f119742e782012a8110fe4a6ba6c7842cba777ab9127065c45a84262207f96973a4f78f58498e0faf411d722909b5fb09e442" },
                { "ko", "818b270d5b872c1de5689834ceb85e78872309792c9987818e28609949bc75e6fb61a1f1ad6d32979c4e8d84c12ea361363b04b62413a69a13c2de851abe34a9" },
                { "lij", "8a802be32b10de2cfe3bc1914c3b079b2febca1758ad2e047b1ff90237db329da47e6cda6aec144cb1abb42cfd7c5f0ded72575775b0d865f15fe69faaff7953" },
                { "lt", "64cba5a2021cf1295c612353d7b44950c2ef3aad6f0c2e994ffd875d7be04c9b15952d1e55eeea7a1eba299f3d0862f31f4931b01b964b52d7d3f1be75b8e0f4" },
                { "lv", "76bfda9e003614956175aabe69c195b50e11573cf876ca99343d99c0073020ef3422485d214718c399aede3b6b6f5654cb4a8cf1bb11ba0244ebea19caf5479e" },
                { "mk", "b9d2311cad9ef9ae926856401c78024af07b2f3a09f18a6057ebe07c0842b0fbc458705b3eeabc80874d97a637d0ea9623767fe70bf0b5af1864d9fd75b21e8c" },
                { "mr", "6e943b10c903b560af67ad0ca2004ec097e1972907953e673aa7ac79b58483af5888556c30115df284563c0829552ce5fa05c16b9e5bc4c3c1e8461bec732675" },
                { "ms", "8fee27881f1998ad1f67fa339c7e2c976b0c95b837dbcb6e3e599b804af9a33e33571118f9ffcb56930ce7170639eb94bbd93072f5adc9fab9133f843e29ca22" },
                { "my", "a16cea3f9b4574a4af42d968d8c672633d538225244e107c907623447a741942130341ce242cbbfda969d2f336f97ffe9931e11960f9e831f846f04c4db30368" },
                { "nb-NO", "23a587dea76cf32aabe90f24b6a71775eba4c28d58484d2dc2d30be5ecfb18495c2194245c0442f4225e197e16862514afb9389be20d268bd10250fbed3006a4" },
                { "ne-NP", "fac9b42f5908d13a504afe70d9d41b0df1838372e6d5bcf0049b729dfdb3f120e6ef9d71c75652fafadef326c466f8f02515015a37fdcc4ee525cc86f1624ba2" },
                { "nl", "dc68835dc906e3c7823f77144b6c1054a3dce13466882eba8875b2a99824e12159f3e9f5ccca18c42f88de6993bc9240827a089651a3e8791792d6f571f941de" },
                { "nn-NO", "ad78e8bb66271b31b33259d3c7fd992c471f476481ca9992afe449afbfdd2ead98bcec322b124ad7cd8684cc30e5e5262e432ed9dd20ce26ed0f09ab0a61e0a4" },
                { "oc", "64afec84e7fb2cdfb107c4637e87d9065e99d9e68f38fde3dfad912b6a52e07c8473cd462b371e8ff4cd9c3df6009d292527c817239ea841d55c8642106286d6" },
                { "pa-IN", "1581766245228b5a09cb0efe49291f848e00699c38601046cd087e660debf6e876d3566982219237f26dd19e38362c0ba49714e80c94546ee362612d692f038e" },
                { "pl", "7e8e42406fd4ae48456c466063b88adb491500d97bc9a419dd405ee70ded7223ba85f70aed02edca457bb14c24482221c54865012d57b1782d50d8a81a8a3063" },
                { "pt-BR", "eff665ff57c466e858ded137e25580df6cb2d26c38075f2df5507b14a715332a765707a82c40e3a8e4d4da878e445fbf309792a58a3dbef0fb311e3bc7ad9e81" },
                { "pt-PT", "7d2ffeb6dd949deb79381cca70c3ba93bf5b86c3245a78d6f222769dd8965cbb1e5ab912831c52093277172165bb8d193328e41daf270f227f9bcf4b6626bf05" },
                { "rm", "5bf66a32d66e22b24937bc414f0d39ebddc6ad33023ba4e9f7b3b22badffdd42dc30c918f4505d974da9e3d15a83977810f9732b50b283a1d8fa00b9b6746f63" },
                { "ro", "d47b9f288e47315b06fb22bf3a0d9768bd7724f02a0805338fce6c8ac452ac316c3de3cbc48740aa1b3d12635cb32d8af56b96bf1d766b04aeb4da52d8269379" },
                { "ru", "d3ef06517c96413a51345c8971870bd390b2b42941726b19b58e0f9307f8ce47dd69d8821012be1407ad7027095a0572387e0146a7635bedd4ba828a33a1a067" },
                { "sco", "ecd5d864c97e94d9ce6f599cc5b7c385f517cfb0cc7bdd8085caa111581ee414def26bcc5e7ba7463f126d916d602cae09fded67a47382cbdfa9d58f03b5f5ef" },
                { "si", "eac3b9fd68cf285d0d3e89902193258a0cde9abfd1e0558d840a04334beacbe6ed422e962b3a8e1f807288ecf32e3d645beb8a073adc68936e1cd124ce0e7fa9" },
                { "sk", "bbad65d6dc018e404c7f130f781b9433a78c95be94696910c3369846c3f9b2502535856a7632e58739af495bbfb11f72df24beb5edb3c8b52f6e8f8cc7c2ddbc" },
                { "sl", "933700a0991f7d3a9a092440ffd66cc00a420008cf1e213420d994fbff81d3a15ee324bbf986c25dcaeeb5cdc2206ce955e1830468c743b0ade396b53e030fc4" },
                { "son", "34dc37c206ffb60a2dc17df57031784bffc4ad0ed211f290eb319891745b2108270360bcefea278775db52459fdbf54c74f7dac6236c7c320777aab847e3ea96" },
                { "sq", "184e764b9a016dfc666aa354c565cc8c5d8cfae41bcf00553b06763d0eea9d9b2b206183db3a45fce98357905a9811e423f8b5668ac1703e9198c07ce87fa9b1" },
                { "sr", "7b5b8a38b2ed6592d3d1dbf447b0d8ad8746732f316cc1d6824b5b83baecb7b95fbe809ea5c8b3c79c64260f01554930db6687f6509da7906fb7ddc45a36697a" },
                { "sv-SE", "252bf5f0fcc319901a9fcc92e9bad43c242b47e16fc779c60546bd181bf91b253ae3d339a95a1abb5b248cdf47187f34cd5f88fdfbf40dca8ca10278b4f53808" },
                { "szl", "86106a3436f63552573a760c208921b2deec08ae55ed83fe7c4ea8137a326eea55639ffed08ddcd9a037c6a412db3c591242303b813ad0ba948548fea9e2e223" },
                { "ta", "205864cd61c940c039ac21282f6aec181dd477b78e3cf16789bffde4c0a229e49f86a62424a3113e4325af45875fd640bac93cfef7aca81b9a58cfb4fc8f377f" },
                { "te", "f39e7a9a3d12dc40216e5a2dc7e47a8780694153b2c94db46802860b4fc498db9b4714e3f5d31c3c18ef86857c10284e9742d464acbdcc7de180a4b71755b704" },
                { "th", "eb3b6a9377c4aeffbb576d87abdf242d28e791fee8a1a3a11d7192974a1ad5e592ae2469661af2c2dcc55896d75719e4679475e198f9cc5f23df17002a3e3767" },
                { "tl", "04e53714d101db8101141a6413b8f7fcfce6a9f95016b264d11734e3d39b37c3491247f681ce3597ff64641f72fa33c2abd2d6115d410ed824e34c4fcd2d0905" },
                { "tr", "eef7fde2e2f07d1c2727b137d60a71f95c8a0e67b906ee17b299269fb670676b0cdf95a196299795c92458a9ddfff0254a2c66fa90af42d3db1df0c251da17ca" },
                { "trs", "003acb49fd12433ed6b42e372497f3b67e31ed5e605892b84a0d0b46695fa9471aeb6e11846b05af2ceb65a8dac4b8028ac2d26dd486864d9b231184c92d8331" },
                { "uk", "b4e04d02c429e0b3f128308d38959b50c4b819688dd3a57a6b3bc7d7cd73a553432279a5085345629a51448c3b60ca517918083cb7e6cc9e973d32e26d15fa2f" },
                { "ur", "1f70b3fa27b68f45367a477a9b7280a3266c6fba306f596c062a0277b3de34b15f63dc4a47df9c5a1c311234f8006d676320ba1f715230602b6577e089a22a9f" },
                { "uz", "2702dc64a573e9226e366c19f90789ce65e11ceab834baa87093b121df5a608beb588c3a4e396b86c1dbdfaf6fb70ab501409675f9d9c0f8c47501406941d256" },
                { "vi", "f1e13bb4e8e4002b27852c5c4b6c1b6d5bfeaf857c7d2e06c6e0ff9fcb92800cdc70f2d139f50cb06f9f82acfe7b5dcf7b2a5f408bcbf80034d55683f47b8468" },
                { "xh", "755fa96966afac85957c18f84cd78f99fa67ba10b5f78f301cd78cf775a521883921ea64e83554956a5b81c658edb93045c8d0f07cbd86ecbe0a0bf8f187e352" },
                { "zh-CN", "a18917d77bec07cf349910eace0c59d302ee4ab72eae251ece2b51b2c14130b70a45ffc1dfe4d213b1b1ab36cc4c421a23e9d87a909f810dde6bd49c31766595" },
                { "zh-TW", "8892cd57d902705d955e04a109456ce1d6acd0001963d5b2e4a0d043a7a73bf1140037440d416fa780bfecc32dc0f7b283f582255e4776a4bc8dba4c5710b5db" }
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
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
