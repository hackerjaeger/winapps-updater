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
        private const string currentVersion = "91.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/91.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "f4a9dd6b5d96c9fe72434d03a7b88c9875ce7b8b789356b2671129c11c01788aa472d86f96b6f7d671d7398ed55f585b8b130dfde1e3045ab9d2900ce2454c44" },
                { "af", "175129826b1bb67d25a19f0dc430a9a57f5449edd173fa53e662680650e73f1014320dd6f757c6b27d551a6fade2858c694a2882345b18c264bffe209699b4f8" },
                { "an", "239b46ab8ef27bd9441b0f0848cb269bdccc424abf845680739666fa979ab9a0a304633f80f07815b021dab6991fc8e7cfba0a5394a5bf3213c63897b75edb7f" },
                { "ar", "1b704d1b3bf441b9fa2e96adad1e781602dfd28052295d8fa17af7801269242cff810ee463638559b091c957f84a7404d64eabbf272388eeb40ad113f7b35320" },
                { "ast", "fb6c2c0aba5076b2f9c2d73e2306c7a5b5a1a7072ffac40313352ec264426283ae844de9777c6ae7e58f91f88d9411643645560cf3eded64ede997125f713266" },
                { "az", "4535f74f008cf839ab78308d969655f97fd2a04f48c0b97b6647ccbe064a1e599fc35c92333958871a43d918f29c961840d68ced21a94c400bbe8b2f37e9db60" },
                { "be", "b9631eb2eebb6244aa2a2d69082947192b32ecb5550fbd43d0a8873c3f0744d1546f5a0e7ef30c561e0cae8a48e13e2cf30a3f82509ca88804e381910c4cbce8" },
                { "bg", "25230e1221ce057b1cd09f28ffe975b20e054af7b1db08179ee7250acc2fdb09381bf2d94470640737c387bbf19fea44238be6623d66c853b8c2c20706f5a923" },
                { "bn", "a2f539b8a13f019d439476b0c65b46a8aec9c5f75c3134f4f36c60bab7a6c151dad99e3b5a85b32acf12aeadbd4d56334cc74e16400a7f4bfba3fbb148d32618" },
                { "br", "b2c60a31df56812d536ab8cc7707c4fd718654cfc14c5d43744529292a89a1c08ae5bab171bd2f06cae085b70155dd4589315fc8d369e63405d874c9114f620e" },
                { "bs", "689c5c0b1b642da14df868d4f0a56b816e9b7532711ad90ec3d849e3af4a9131336e8e81e5561aa65ad274dca15ebf7c62c5e05d6e19da6f06e865554554da27" },
                { "ca", "1d7c58e178504d46838aa3196edaa5b359e997c713787d5bdf7d79212725750125b8627f4b55bd5bd8345c98f7957172b56f0a0243c285763172c70f434b8470" },
                { "cak", "6975a4facc770808c16f97630c17ddea37f318e9063121b37bf0d54eaad32c4909fe1fafcc4126f7aa38273f251b6d96f823131a02f91fc9941c3576ae3699a5" },
                { "cs", "bef06d70c3843cd524447bac8028045dfbee0178fb5369b8b5fa9d972f48f0fd44d3b2c8268d703e6e28bea6b36c96b16aae6e7593d01274abf14d7665f62405" },
                { "cy", "a04e3e448820135400cfd61099ba0132d712f5877d8a300ac6623a70488b8d442eb95bac6fc8459bb6c4128db7ca10c9fa0cea27456af1cf71d6e7b15221b0ae" },
                { "da", "94b6ef6967273dbb8a92aa245774b0cc1e229f254acd4234d6b63d3e7ae755bd1a36c4d574593f534dbc875404a2a2daeab697d71419747c4343e3dd0a72895e" },
                { "de", "e081dbc3142dc10e25cdaadd31f5f78f8c6686240762fefbed238ad0c777e40ccee685b8c2041ea7cc191099324889a02e7124510762fd67d906a307f14ce0b1" },
                { "dsb", "e0ca2c38216a15902b7fd569d5e148e36b5c7af56595aca129a51339a96cfa00df0d3aa4f1b6881296dee30a4e1f6068322f9b118cb264e703167ce45d0e5412" },
                { "el", "de14329b62dfb6d8e0817bdeadbbe19f274007df285f53ef7d29c0a7465f8a65a647e8f0929b5795abdecb146f9d9f5e43f3dee108f376a035fb40a6b14fa19f" },
                { "en-CA", "a89bef465249d2b71a3eac5166c0801d9dae0d0116d04ca7074419b6806dee016051561e36a32e742040efd551e642f585a730db3e187b5e3cf793ce0dcc33b4" },
                { "en-GB", "22929284324b36e94af115e9edd0d97cc86799b261a713c4032aa96d9a4774041d15dffc7c392e8b0558eb7120de9272ef93440df3728af12f4dc0c8e7403f14" },
                { "en-US", "ba68c47c20db75ca5a07ff877d647deae1b876a40321c98e5f3ed722a8e6ff359dd002050718fdaabf031336b44229ddc0c488f3ba1857a2252f0d660ad82461" },
                { "eo", "efd103ddb1942671143fb0a5e77218baad547fa0c144f2f461b7fde9ba66fbf7aed0f5ce8aaea33aa04855b8e111b263752b629efe1b8cf661c31a6f7049204d" },
                { "es-AR", "6608aa9b805d9dd7f4482cd6be2fea19591aa4341499d0ff13e215977978ff8fa1bf89769f4837b237903740292d978388888869425b1c37c8b4c961e63c2a4d" },
                { "es-CL", "ec94c5d59648ce51cb046cc11244902c2dcaaab09c1455e1b7d32f79a9c7a10057d42e315237747431c742f96f61d9aa9e97a2da686fdcb19b976e31a0e67ff3" },
                { "es-ES", "b4bb7125f9179e572c26975879b0fe9f2c51747b8942fded03502e6c7686257e2b700af940ce53492d87f81b3ff97c51b506b25443dc281a5b04d20bdcb1be45" },
                { "es-MX", "712ea6c29e92dabc898388575823b79d95cec549089133c899ec1deb81a97ad7f3a961ae6421425a8d9c1c89582a474a8896f60055dffc09c70004be2bbe8e46" },
                { "et", "35c5b362df7072ab67a5baef500062e1d99b5705bc1d5ff9c390d68df6e7e99c8efb85891cc8b332ecc809ab20b5fcabdf33df06da030961409c2ed793a98b34" },
                { "eu", "3e34c856c3ece7bbc801a75bfd103586904884a1560e3f8421dfd7f72b4a6b6ec0441edc19f1109880cf9824286ef923dfecb3ba88b1e29917b8a0a663a768b0" },
                { "fa", "1971a8737b27b70fa1cb4a463846f1c4b7bc93a97eb6fa1ee2d96b019ba63d3815c10aad28edf239cac310ae70c764ade33e1d20f2e1362a6b294e2e84c1fbe9" },
                { "ff", "b81ad243b38e2cc93e75d1f1562d53f4ac7056f9794405e79e428f0a8b5174737ba8a732f96471a169a77410a34421a03b70681fe5753b5d4d79d0294d6c3cff" },
                { "fi", "3d583a8441791a35752feeb9ea399018cf5a982b2ce96ef0a13a9409347345695a0b9878229ad6db0423bfede567d037ba50a74b45890a5fb77cec84322c142d" },
                { "fr", "cb8887719f850cfe28425c7924491a0a84c7e1b40302c9d8a743f65af5ce2e1d1a2537d71383e7e956420b28e3a14da2fc8a7db386f5a8281e97e599662f762a" },
                { "fy-NL", "6b38fb2c3e297b1be992208b1f95b079277561b276925e92a4e9fafb6045788c91fe64929b996f160df943c2c81e940dbd8fb7314a9ef19932640b207982e372" },
                { "ga-IE", "90c6e7712129373d0de74c46f271ffcde51cc29aa10e209956e18fd72f0df325ffb3b27ea5120d58ca62643a3ffe429561ec5e2de3efaa13191124ef8512ca47" },
                { "gd", "58da0cb4ecd54d781b83d922d6860d43a9bb9c88ae30cee078bd4608cb5a31745c40188e95f535194a84cb12265456f0f3ba4b32695c97cb26b05a944860e97e" },
                { "gl", "a921ab4fec6942cac4f86e5afaecfd4dd45df398f862bfcfed23eaed3f266e81b0ac669bef5dd7492a1d1f8bdface0cdccea98ff2458dff3c345b50438b81c16" },
                { "gn", "0f75352f0b43fd58142beefb55aa1fc1a3f5a5beaad8cac4507c142350d90296c5f7f78a771ea493fb8622f20555c592d3d5bb9209a9683ae4b01e306ce40742" },
                { "gu-IN", "fdd89f0a85e509922ec563c059ebac2f9aac4dbf568333961dd1fd5214234c229729b492a64541ae0c54ef2f9cb31501a54697de88e5b7f55f8b0a5772b73c09" },
                { "he", "44678e0d92fe6c8da6dd1443950196a13ced07f65e479b1cf08319333d455f2326de0a8bbd86ea6a6f8d40708bcfdb1fd8cf9e0c023175302b620ada81561713" },
                { "hi-IN", "d018cfe7c07e62d15df60e7571a6bccdc90245d25598e916f02c8ca2c54046765350175c792f0a57266f659ae2b9dcd8c6744064ae2a19af612b39809b8d6d50" },
                { "hr", "946f7aec33f548c9d4eb8755f3c352d0ff6b22cc69bcb67d7099ef993a83bacd29e9520b3edc72805531822062136e38d2c59b6ef67999d9457447cadc496d63" },
                { "hsb", "d16da350692aa1ea2a2a67b7fea8018338554403efdcee5a221b83e2fbebe2d5cbad59d6bf502f6b576332c32eec9acf0ba63a97bba0758e83e4675187925c2f" },
                { "hu", "e438da5b814534ed87e5cf178d839e89fc039e4479d75db32b259c47756edfd61be31caaa752a7615788ddb3a3dd9d94653f866fb7f64a892259a9672c9b57dd" },
                { "hy-AM", "3bf45d923bd5534cd3dd62cf9c9e7a9cfb33ecc923e44007244f2616e78eabf2959d491b8551c13e48dbfb70697b7771bc31a0e378542cd8bcf49544ca9726a7" },
                { "ia", "95e7dbc062bdd2ff337d30ce1ad41f548daa4cf552ef8076c314160daf701765eb28c84b1120f32ef6e935dd039a95b85db49c1672d43aa956f66047f5e7f53a" },
                { "id", "5ce5406f8293c925cc9f824b7b9607dde8a439d6c877e5b2aaeeb6d9d8e5da195c2ef0bb436ed2a1cce6f508e9cad9f0df93f8cb56cdbbb7600fc4d0717dcc50" },
                { "is", "ccbf611f1d3227faeab6915778f13d3ea6bc83fed2e271fd482a147a2f427fa931e7c43a76099542dd7c3dea8f72133df8ba557cb0820065788380cae8262c92" },
                { "it", "2b22d467d837a8f89ffd303d729af6a7b50fb6938b7eb6659d6d56815210d2afebd50c91e96b6f411a58948188d403871a5fc4472a80cefc1b7dad70ebb1c1a2" },
                { "ja", "09fa0797b27b488b737efbbe12588a46f4c8169250316c6ae577fa24407327aa3a3504fafdecb88ab96822a70afbd9a25ac02c6d0e71d6c6716373edbf90037d" },
                { "ka", "3cb097aa01181ad94c68b991e51435d8f15201f66bb941fe71670e62059d32ebf58d01443c8ac39b6f01ddb2e3fea1493bde8319267753da1ca8854e9d03b74e" },
                { "kab", "9c35547fef813493062c5f3c9a686dab19b31f39e1641fa3c3854ae61fb946ee7eb6b0c51fa4028956d59f199360e25509eb9bde47e6d9f6fee0b0c922385dfd" },
                { "kk", "38bb9ef299d81e59cf3bcdb55849ef6169ec060157ea2048bbb7d16e96332384e01bac68558dc5d93182011a255da9330ced02438f1702b8139886d2ef49ba5f" },
                { "km", "41106c603f8db443032b5aed1ee6e89500e906f42ef9bc7915c32083769c32a6286a070c83974afcb6fdcc6ea1b06ae203ed9eed3ce654bed56b43ca41da8f32" },
                { "kn", "253723dac179799a1e6ee080bbf6dc9ac1d8d776d0e3013af3ba7d5bdbbfbdedc83d2b9a00eb79152ace4b39d60261b43ca523b227f6de9f99cb0796c00736bb" },
                { "ko", "748fd0a9160f0ffb729ea7f7d4437a926d9ad544ccb6e6152d501bf9e51067e7dc56f87a6b76838c3d908a83ef32f3f8981aa7281dccb9609890a68f11010337" },
                { "lij", "dd45ced5372e3d0dea7801cacb31921de6e99a8fb05148cf631f51d388c133bc0636b0456fc74973bfd10c46c8eda0573a0d0a3c78cf483e0013d6f46d542e10" },
                { "lt", "4dd0c6de4235a794113eba8d3a626632c7364ba2852a8d1e8db034ed1510970a7571d8c2ae1c503a39f7ff0f9b0bd21a908e343151245a53bf219e84458fcddc" },
                { "lv", "9e3d8df2dd3fb61ea5109a2926af86455d889135185fb03a29cff9ba2babede5dc5ccc695ab6dbea283891223fcbaa69d4400f9dbd59e4e164144156f5f0915c" },
                { "mk", "2cb3407fe56522216f96b680b78b165e1db5803d4a7fd3d44d99e788e1abd9f877bc9f03677db599c0131ccf28f27b0755a33badb9b96095cd301d3a1cae697c" },
                { "mr", "d66475292c5d717ed7a272ca3e6b14af4f310477b4dde482e470cd71927d003e94eac903e2cee7e8bbd56b49d886df255ed18fab64839820218ea62bee62e7ba" },
                { "ms", "4468ee7924a5bd0f9f48f8dbfb9f297e3cf1bcdfb7359078baf71c983a575b115bf2fcbe0b260c66933b393bb227a4cfb2ebf6b99df8038f27a4a062e01d4736" },
                { "my", "f4985ea30acde66fd511a2ce288cda1de1c0f458c68616fff412a557d9a3482160b974ae873248991ce073382db58c9a00450fa400f24368ef85ca8e16d9ee57" },
                { "nb-NO", "e011b1e37809c7f9574f2369a1db314b0c5227e75d211026f9869406b797620a6dede62200e0baafc70b5cd3356909597f1a74b668a7d0ee1ef3019ed79d70a9" },
                { "ne-NP", "7047eca49bde486b8c2937622fdf7143b0d1cc6c6e7d20d7e4ca85ded52853a4936976f5f7182dc9a9519181413d50ad7553da0d03d36f9d6fa1656fc80de521" },
                { "nl", "b21034bddc73a133046056498d3341753ccaf4c0a01f5aae6b58f848081a29ca55532906868255008c40b0405126adef1b0f0c0026efa30ba69ee83fb91758f8" },
                { "nn-NO", "ae23a0689157efefc87f17b00d1ff858f3d6c82780188628805c0517147c8cb687707a6c3f4484fe1a4c13de5e2c7395bb55921e7c042c6dcc6945c2e2fa5449" },
                { "oc", "69b86d3accdbeaf7c54e08e9107b0cb1322d8849565715ca4b28061fee62d80d79241fda6d0a9beb782bcdae1ab02f83e96fd0bd889cb1bba68ef5780fbe15e9" },
                { "pa-IN", "f48c21081640c2df45ec0dea5e59052fcd5b6b1c591271b5218b30a13fb7862747a211c31f2060b112d648b7664032994e36a03f5fe971417205c6d9a86ae8ed" },
                { "pl", "4d9954aafd33734b0d6f404488cd266a9a9b2b30b0406df5382bf9cc4c083b5c37e9d185667a901d4942a66d6390bba3d3b52b70856ed0647d4c4454aa859205" },
                { "pt-BR", "745687f6c646f16f79baee608e379cc40be7ead3a06c094f745d6864e75a5d32ef9c386f5ad2458d19ca56272ea50f0bb049abef4a75b4081421dfa96c92a27b" },
                { "pt-PT", "25ebf45deb58d9b31e44dc7e8b59a7f1b906928148d47ea44711d946a1cf3db6cdac8403a62c53a3697d9abac8331240cedfb782f4081ef90a40b564f1cbdac4" },
                { "rm", "71f4d6c5cab9961c5157b31f0e0d62791c9104e84ddf1607f374ac102d1db832e12ef6f510d4c6054de70472c2c6a16e5b3c6287666ce826385d4d0e1160b3b6" },
                { "ro", "0aa721f4099640e04e758ab60de22124fcf2eae01d15b029dec3d2c4dca0cf23561762f6eaf7eb53bb3394040ab52275eecb65493576863b4de2734fc9aed582" },
                { "ru", "7b0a288ad69f3e6419bd1c56b17936d0911dda38cc88f0639dcdadca38f1e35e70e8eaaaedb4e9b1618257ee9759a4fcd35b11d125a9d7815c3ca6408dbfae0b" },
                { "sco", "f1b80182a95a1322088b4909375b6ac21eb7c75087f3737984e8cd4d4d9ed2c57a54ea1042a8faa363ab6d0fed8ff21eb25bd26434f47f67250dac422134052a" },
                { "si", "2e3260841e80924ff097fcac1945495db5fffc4e86676b9e9b1b4b3dda0b2ad3317d8970c0c69f67d01c76f28a82367e0520addd62c76aa59c112b8ade72a6d6" },
                { "sk", "4acf394233484537583b933f800d33a892248a803ae090d4da76021e09c923f3a0f684b685db8f0da582fed033f4df1db263d80e8692fc4f913de45ea4095dad" },
                { "sl", "f64dbc833eaafce7703d18348838d0c1213893d8a8dccffe9d4e53af18696908dab5301268eb00b553d9640843fe2bfe67e1b36d347c465c4f08a5ed91b84563" },
                { "son", "cb093714ed4218276c1f2a3e72f3f3c5ada22dc1497fd699f9bf41780355f0bd5ba2b8702b0a059fb107179305ab1fb0d7b570655d0f38b88430415f98ad439c" },
                { "sq", "7f454a2a3cbba2cb4dff9e4a390c947c1b7ee2ae4413b21c102baa31b5135a8129d3d17f32397140bd8a4e8b65dc69076900ddf646f89b00af11a1eda095d5da" },
                { "sr", "07edbcb97d9c3f627e27265bf94dd69274423a3b7b44e4ecf5f8ac63cdeae47446aa0342926aec9f4142fed75174eaf2b539fd96c0c662c71efc036934d62a18" },
                { "sv-SE", "70e054cbe0be381340471d64c15d9cf75cab4cff8f48e9f263983749d506e97a1744a27a5b091792ae97e9e023c8963c554c337d7336bbd2356b188dade78f28" },
                { "szl", "dd6ac5c9efd8c514d7ba96cd056ff600592b159b3d1f64a06f240521c0c6c5c1b0e6867cf6242ac901ba85c542b1190f711611ad0f79d0bdde4e70441b372daa" },
                { "ta", "a7568aaea36ccb66bf9ae4d9ef02d800827f0b21b38240d4cadeb9a9dbacf594c6f8f6eb38e36c3220843783c6c5f91afa1787339b5214c4192fce57b3411e88" },
                { "te", "08203ff2120a1a1257a59a75c3930163b8ad9b6fe636db297de8950a202cb760ef0734e2af719a79ebc8e41e49c2e911389c60e8984d49736493df99ada579d0" },
                { "th", "5dc74ecb886d265aa61b99756520f41ee8e7c033e68a4b9ee9463220486eecfc79ee2f5d0c866ef11951805a99d5c0acfcd15c5bef0f02a7a7ccc8d7366a4d1a" },
                { "tl", "c38c401af48e27fa35d7d6512115d1a93ff65d37eb7fc8b8d933c4aebcd17cff895a1af5a4d0e2744903b48045a9e3bcc89dd9b70c8f1785ff98e6de085c0cf8" },
                { "tr", "88674fc1322fafdd9f565083033265fd830ce0d1289bf5e375e29075bdb63b869b9e8e12dcafddae1078e76c96d4d5f92170eaae9cdaa0a23dfec2a4af2cf3ef" },
                { "trs", "2ff4d9f1af8b0774a7ad44be2005da936115ec655481af0422d58a8246890794c70d5e21c7630216651b2addf28bb3d6681171a3f53136792c2a18d2d85467d4" },
                { "uk", "cd1c049475e173f187a538853c15296d3bee61d08d101f8f4c8ce2cb006151aba48fe99e856124c3102ecbb1e810ac72583c2f30cd427899817b1423a59965fc" },
                { "ur", "c4bb7cfdb28d9d3145fd8d3098f8c6fee295d1d9579470dd76c462f5341894eac9a29512a8bd49e7b6aea8a71f91617d5b3eab3c9a00524bba352d52b28155b8" },
                { "uz", "f4f1bcb5009742b70267d3de0d38ec70c7575d0d3e300d0afbebd2d43ac8e07251d9f2eb4d62e132d8799eda5a8a93bb6c047aae6a61eb6fc5ab2a6fdce92725" },
                { "vi", "117c71c411012f258fb6925ed16d71ce4ddb25ac214c10f155b3f3db4969dacb78445183817514b620e2b7f6ddd0c391fa62077b3cb2e7103b3d92d42511c723" },
                { "xh", "b39dac340fea25ab5fae274ad5932ba2349660e72fd3113e58ffc9ffeb8c4030b75e89b742ef351554c281ef028a8749092859368aebfd30105e787aaead1490" },
                { "zh-CN", "67c32afd846f28f36732ee0d8aca13fea9a62c186e2391ce97572e41d5c6aed8f4a3d3f72708743ded6c51cf5eb6baf358d542a94ca336b328a993827f06942b" },
                { "zh-TW", "2810cc5ce55946a5e6e9a3d434bd863de409e4b6bab5662936cd2a8c2d47fa9b41e9f013b34d893a460bfba16f80ae53227b32ddb71be05c8991df8ec476c979" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/91.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "0546887b11a88e63998f3cee971d209377b7768fa5d8cca3e7ec1e4ed9831072e68000b253e70356d9e8b3b58b8d6aef6fc84cceeb29bdb4268d906fef2a38b6" },
                { "af", "4685480b4f90fd4d3cdec22e980e0493c473a38d9584ce2f70701c3fce861823df53120ee08ebb563ad0121aecffff6b4204687d594d2d3e1e2f2617d6b11c1f" },
                { "an", "319deb1aa155d3d82704dd0bfd9bc522d16c8dba43c27f4a3c75b9a4555d71a4cd714ed1e2483a16c3847292b17a3a062f0311de3acd2042ad0f1eebfab3d956" },
                { "ar", "07aed558e0592bd1e28b95e5cfe96c86d2d45ad3a1832a98c358fd1ad948415959ab2900ae3ae1a537421b934202f5789d2f2311a0a6289987c9ee8651f2dda0" },
                { "ast", "cedb86f4880565d6ca63f9848748d02d4830bf68a4b6f9dd39767b098ab0e3bc11f3204ddcf5dbc02b1515cbf1f0d428997acdf1eec48603ff5b1082dd33747d" },
                { "az", "f8a85bd51d923748a70ef7acb73a72a85ba33208416f191b7f7eb72792f3c0bea7b0616e5e0580c29ecebe5b544e8de2637befc3cccbdc7858c6fb2f8295000d" },
                { "be", "21609a10e407f2c026048df09ea202dce3caf4a8137bee706fbf6203e6df21492003af3f1815d3eb472ff3e7427cbc147f3667e46c002672b54d73e96116c149" },
                { "bg", "c79ee0dc35a96f15420ced354ec151c565af75a65960be2b41ed3171216523f8845e662b110358c26858ec3995a7232d3a2c90d4f10d7edaa711b46a75100e28" },
                { "bn", "62dc7671f1a7d7b6b5b474848da68c97dc082d2f760804a21f045bc003d41106dc544050db716e983edc54cdd8d7870b66ed117754359ba574d2ab176a5b6d5a" },
                { "br", "aadc278ec3f043e7fad9ea9ced3a3aadbb4fd95eb18e676869f6334c9b16ac22d540beedc2210e74917ddb8e139e23984cc72642c46b005a5857728b46e9cfa3" },
                { "bs", "87eb471dc96ca2da94159bac4a4522e12f56d652736a9865bb7e710936d14c6caec1358dc84292e61f6fb8ea2124a2ee39a346cf84a58767162e8a8ff2d720f4" },
                { "ca", "1839f217bf0a93aeded393a88674afffdb15e888e095bf8e0ac9db08d5a23054408321e80065d62d88adc4039b9160d5f4c93cf4cf43e403fd618cd098692824" },
                { "cak", "6feca6bf6d717c453d6e343ae01795d1c60b0f6e6e442f2e8b970b71e5a18c72b06140556cf03706b4bc553986376521ba2d3cce3af7894b2e70f7f3435db8eb" },
                { "cs", "af655bb2d0e2722fbfd3264137c8a5e77db91fb173d54952aaabf03c441782fd359ff8a43becd1d8fc9d5735f156cbbcfde7847a33a1e4a84e3f34ff44927919" },
                { "cy", "19272ee6501df62139a432c32c313fc44c2359f9286d19bb197cb9b2322cb5f0dce6981f2d781f709b03e199a9fa9aa3dd05cc2045be56d7d091a81eb0208cf0" },
                { "da", "93536e18b249c2ffed4c25b8be20068d36f7988a1d1927f505d8290e02d0b18dd0c74f0668fd9da82799bfd1164de279b591865f15ab968471d61ee1438ed7b8" },
                { "de", "c2aaffd0ab5b0918c6a7ad7d642fba6a4a8b127116f52a3b5aa0489680625a67727a924d04e34861d8820a9cdf96c852b593a6d8c28af96d420d483e812eed97" },
                { "dsb", "51f9685a0c48a42b3238fe336eff89ca86fdb3abefd648e5980795562ff24157a7d0cf4ea75d7436748e7bd806eb94bdf79b0b3a86baed48966673979a246d81" },
                { "el", "5f813f082ecbe689172eecbef53dfd6af6e01380a3e655571cf94df3d305edf74fc6921983447962bf8c49138b13cc9e502cdb3a36aec73a87d12af3cc10f542" },
                { "en-CA", "ad35fd9398a6d93e1d3ac4f122f080f2fc256c9628cd002b08d9827d7b320d1111789f78175a7301197c4e91bb0151e16ccbdd0a827df29a20947c44c18bfb88" },
                { "en-GB", "1309f0ab5baa500b0cecb95d83c4dedc5f310aae4d8b8fbc73630c072dfb20a8568321b00b863ae01fb1ff461eb9554bb115043977e711a377e2f37838946c8a" },
                { "en-US", "013289ab182d523ae69231a3815002d28f1a66f6bdcd8f74a0217e7def28d5c8a07efcf6a97ea3cf9ecc31bbfa24750a012a3fa758e16b8721c00026c2d04353" },
                { "eo", "30b1a991370c6f0d0ef4e47595f7f5d476f4bb9dd05b3675925fb1e9c6c8e24ead3401e4d5bcfac14f5e0a0742b58c3ac6667a08783d6e50444c8eb27134a5fb" },
                { "es-AR", "3d149cd2c927bade236364f9c5db71b7c1edf7266969ed8965aca1920bdde66c8869f5d6e490da4147af741ad82b377d95b0e2bd864459a0ba57db61f6abc6d0" },
                { "es-CL", "109f28f992390ecba65e031d95e64078ed66b39b4fc173bd7f0864780f60df2370702776d04df1b4ffd5399d01626974bd161ee65e6c1003fc9507951166f214" },
                { "es-ES", "7a1d2341a9568e97019d224317d80a2796404ec7b70a9334e7dd8d28c2f9565b978741863e9f8485ad7a8a2140c6be3c13d51c13211c94dd00e91bf140bc60da" },
                { "es-MX", "80f3c97ffc0fcd10d6f3f2ad4e5b2c652595bdf9683fcd8a11f9a4bdbcf3a024159cdb211faefdfa1c4ac3551f777a09c6ca2a07304ca05b5ad6bc1080b49a71" },
                { "et", "7fa3a6f035ffd32e594f6631b8cef54a7dd31a425aea7b6a1cc93355434e8957528e9cad200491db56ebcd369703dc42e45fe2f62bc305dfb72bd1d3d6956ed7" },
                { "eu", "8cb8dccca8b441ab355b1cfc7f99fd4e6cf426e21699603e3f1853f4dd04fc09b755caaeb6bd1b26a137bc65267fe5cd7449c88f6cc12c20fa1e814354fc668b" },
                { "fa", "fe78ad7612218804952c91c804060bd1dbfcb6a43571f6c2e4bdc63e2dade53e8cb3584200ce03c1051e7ca9a35dbd7b463815b460a5c77a899fbc8f7cb1a410" },
                { "ff", "d7b7670370a70fe6c8a752e1ed17fbda128981e234acec959e82cdab514952842acf8a966e9aef5bd4ddbfb52d8b6bba59ace3699c11a02d57b72c4513270177" },
                { "fi", "61af6811440d812b95008e4943f54baae66a5625a5130f32cd468a71ba92c0e31d44d10924e034fe7f030054d69cfac81c98c87da4826c19947d9d6511a3428a" },
                { "fr", "dd08b321ea4ecb94286c7b9103521b4326c431b43bc2d95f261d84662b8de06dce319cb340fc02d62525e2424edb705529a5d68dce492dc908528d98cad53b33" },
                { "fy-NL", "bce1621f3ffce8ee82a801cf880cf2edf9c258c084bfae88a424f1f85cdc8d89a6e4936b9c209351f5561de2b9414e63d4ac45a452af89d380d3ddd53cc1e981" },
                { "ga-IE", "ac4593bb0d2461d2d089f348a722bd612fb84302fd16c735bd372eaba7165a7fc69af6cf5ddc0a3631eed382ea79a05c721b2104cfd9f8fd8a9a75375fa7e542" },
                { "gd", "e29234612ebe7092b844c4ea2f3eec91ae38c5153818d1930835459447b2a03df6a660e2d1435d0c33b1c317e9844b4348f94e4e744db544099e34fdf47cb73f" },
                { "gl", "74ea465780cd5cd565628e6ddec632d099c54a659d3214ff1a2712fb60eeccf1ac6bce3d4d58f5c122756b33e35ab61039940f32d6c2a796957399be6962c3b4" },
                { "gn", "9ae7bb510f7ea677ed044a3ac88e59ee0c08654f03249bd5c032865ea1d69052801536f98aa1e1544691509e7479c432337c4901f2bfb2ff00ff35908a720e6f" },
                { "gu-IN", "c5cc20faaf892eaaa4e63bed67388141e83cc004833a3061f4ab828fac43565727b6af5501961c4b54d1065d099bfc7e79e217ddc05de482093e9359e3512469" },
                { "he", "ce2ea2e0a9539e8ac225d1c1ef531bdd26a0024d1663798217f0c66ca5d077202d3ca33046e73d016c7af9aad8dcbbe63875cd2b4fbaa94a8cbecbdda09b7177" },
                { "hi-IN", "b2ac4d8246e4553b2b367be5189a4a6fc15537d5485352a6694d9019362c5a72b52023ea049e7584ca1236fb9f71d9c40b29b9d702298cd828f2e568c93ee638" },
                { "hr", "45826332b5c7843438647a90a79d6ec47880de7f667219006551ba7a9e0d5dbcbde75b883a807f93852118159bc61ec308f99005f626c26f9ec537d99841cefe" },
                { "hsb", "f05b6fc842ab20823a6ae59b8d978e379f6a2d69104d8e40c7b3cf5b399310b60e4a57c2672d0d92dcc6751ab3e113f0e347858a4e27fca0557301d18884f926" },
                { "hu", "6a6d545f076cc633309b7aeb16b0fdbcc4357a062a817aa0f7d9bd7ab725b72aeca42aac9d8903615f08e24b17403eb62824ed7696e57b21d73a55651c339ca4" },
                { "hy-AM", "68d457105dd890495371232257558ac58b15f2e320a440ef267ab9de30cc377823d3c5073686899c5d4cd10005f8fd65bb61055ddde4add1eaa8136828a7432e" },
                { "ia", "56b1581e4e87900bc7f0fbde49489338fae3c2a8b11d7d43e59af42bdd409c60f46b59d61a4c180f865821cc93262f20dfcbe870557ca88fcf0ce36e4ad0bc4b" },
                { "id", "15e9e1d6bc31b1653203da9a25d80321c4156a9923d32cea9c81d35a12e0152b9a8715d95b58165d2483fc7f66a4bcdd2c845c8b8783b2dd2bfaa9dfe179e040" },
                { "is", "be2f3f82ee96ce4086eedf1580089e60aa150537c8ada4a8a3e4989cc90502364c090801b82b69dd5631630b37ddcbd44ac8cdb3bbcc5daf8d765536a6a5ded2" },
                { "it", "6c7a99a7f0449615a3b4723e23ae0206700fda922cada31b89be4fb8e83caa879fc6bc207b2dfaa62d9759409cf17bc51bff20911b4fffe3d68c01b4afc5f049" },
                { "ja", "62c347e51361abd681051ca315dd4e66f972c22a6f51d217c67b64f7a6b78e87733140260fb0c406701b3390182ccabfa4307102854f4d49448cf938cdd54654" },
                { "ka", "928ffe3842d5d95fff2b143eb07bb070fde90df2c7a557fcba8e28278f133277561d166b744b559fa0dc119f46e1ba64bde115f462bc21f2a3761d74f1fe89f4" },
                { "kab", "6b4951fde81da646e932505d83edb30ea6229c09db32b0fc661b82da12b8f69aa2a158db191f01815f5de2c0e39dac098895c0278d568e56530017ebf1655b85" },
                { "kk", "e2ff1e0423bb613af515609867f7b3ca07ee2d0481d2dce5545b4b2d796bfee6a49f51a8adcab3762bbcb492022962f12259429e9ea7cb3800cbe9ee4ac14d28" },
                { "km", "33b15076615b5006bc4a4b5106e45978b16887cba1b7a4ed8417492dee38734abaaf06f8c191ce583b97a18729a0ecf0594ad25bafaabf8b06e027ef60ccb962" },
                { "kn", "7a7432cb746641791fb876081fb063e6e1a5da3e20d5ed6065b8593bed07c9125f6bd91d452770986f037ce9feeae65884cb9815f23f0e7615f790c61a609122" },
                { "ko", "862f2153022b5238c8943d58e41552c7c36bcc83cf393761006668568cdf5c98dc41aa9a82273488ad0b9149600a358f5939f345bcd4523ba26a6b40de777913" },
                { "lij", "643bca4f3cf5afef43124abe8f7a257ad1102d85fe52df33aeb70e82ae2b4e38dd2bc648334865cf9f8c6591cd3239ee3bc0679d778487d3b43393d6a55185f0" },
                { "lt", "17f563e0672172bb2af54a5a0b4ceabfe0f44802b3746c8971091930fcccd394faf763daabad402627701e380437163ae9953b0783df673a96993546c8f54986" },
                { "lv", "69624058f39f26198e5458e5907b8158ee247a5c6ef8fa969c8668976e934d68b8824ad2396591f3bba4c77e65fbf6c97d7e87d131c9d81f8f72edf4b26683ca" },
                { "mk", "1a6eecc8cef28c08ede6d73692866814a468541c783be8bdb2d3ace0784e8c051b7691ee01ee0d00c4e3801a8723ae7d501208e80de129cd43faae5b449de136" },
                { "mr", "495a415c276cd3b31bf4809ac1d7500bbd0e10ee12b5a94165bd9730cb1470fa9022a35a8b964372e7928710029c96847a450ff468b464c41fb7bc71fbd0e30e" },
                { "ms", "8e07d892ca1e6dcd18eca606461b6ba464ba0675738a457aeba7ab7b3a04a02f4507dd6493928da366c207a0aed40f847e62a51768c24e53c5d047438637f934" },
                { "my", "fae9772ee7475f1fb8d83e73d9ccd37c98bd4a88bb445d73556c90db3b0c7f55a0c608aeea885ca5172399148bc4bbbacc5256e3a0b8c6020ddde4931f4495a5" },
                { "nb-NO", "35c458422ebd696a125eb566b104fd62111c8edc48e6f74f7fd99428d7b524b739367d8a3dd3895be72c30f7ad058932e55ead354394283955f27272d57a3fbb" },
                { "ne-NP", "67297ea19899823514a3efc68ea50fb394d48ebeb7c0a4615e3fd8466bc7a0e093edfe280997a6f42771434819fec9ad4374405e691fc8c2305029648f74c0d6" },
                { "nl", "c06b2bbf86e1567eb208aa246b24e822dab4835c244e3effa98d83b70f77fd8445d8468d36a8df702adcbc6e944a25d5cea9ce3ebefda076f499d3a3a4e12a37" },
                { "nn-NO", "426390b8448e2b333ea41631a0ec435258e1892e2afefba762239802e85e22e8c5337d802bd95f0b6ed7f9b823668e3c771fbb1e84a4220364a8247b8b39a02b" },
                { "oc", "02307162878ae7f44cf845294abdc8c501f7f2abf99ffa1f977fa806c76e2b44f035636e753236689814009df16ab9fa4f6299af7dc16c6e43b9edf8279ecd3d" },
                { "pa-IN", "4030e5e495baa23178e84f1cb670c79268c78660be79a11ab266230ff0267777433623406fa2c6acf12457fdaefdd8938fc749978a76a7ebb98565100da93d5f" },
                { "pl", "4e9b4e4bf45aaf5aecce45752bd19677b2d61c0c8169a72792a8d261f8e9ffd96356e9a1daf231ef9718ea456e5fb5660b23cefec9e859ed9860852a0fd8ecf8" },
                { "pt-BR", "54b6560bff8493b2deb6920f35f7b06e541ee1431768e74e8d25f4429d4f830b45ed6c1d297e62ef7795a05217357513703677f1a4ebc5365f7a8ce1512ee36c" },
                { "pt-PT", "608ff746c30ef3af0c20f601a93849ff605a875b629575c33f57349a33d85ad4eb9ab74e35a0dffe0d251a9d2c7d8a2031906dbbb6cba55d90b1abd23b6d8feb" },
                { "rm", "c078cae78207f4ad23e903cd822b23b5db626bd505f60028737599db2319419f1c87064e45950b6c7dffe3a872e3840b84fc1e3843d0d11eaa1aceb3c496a5ca" },
                { "ro", "cd42b51fe534f2ef058d16f746911d651172fa141fc2269d296366a974760364953c48963fb64e8f54d0c28937346f3284d0536c8871da587d6fb72bc2d46376" },
                { "ru", "2ae9b0c06f2725a4b0ddfc23ac315346e714984de2fb8ba4e885903604165cd1f4ce28a01339163178657123032163ccdef9ee376a5de336998aa8f347028a79" },
                { "sco", "abd30be4502cc19122e3ac33aca645671d87518b74002751eea14a9a5a66ff608f5b63d02a4a56cdcf47d19953127d843a8d025462670dddd1e9ec0af92893a8" },
                { "si", "d170e55ef9f9c626c2527a2a084e2058fc5fa275f63a35641e39ea9b8bb4a1f13d008ec16fc0f081854b0adf0450670ba9c2154b51af1aaa15957f7f9e1a7db4" },
                { "sk", "83333c2bb0f91dd679670e87beb573d7ce5d907d55fecc023c9c8037399b3a58f399245771ad051e5ffa7f56b8c5a12a1a68c5e50e39970f530e33a505c20c4d" },
                { "sl", "177620ba9ec5ea3a3224508d31947b147d7d5049eaf2a377fd9a9f46787b735ccc44ab30bf8ce50ad6aa8be63815fe1179dd6941d64ca4f78483bc5ad1b5614f" },
                { "son", "091ac1010d2c7ce2e7f74139e3752a86f6a1083a5df2d513b02c6cb01fa26c1ae98e4550ddd335f4b72a1b316dd9c9274009b12b8f9db8d9554cbd1defd888ef" },
                { "sq", "979f9e3256b3d49740f4ec53be5d2b794a986103aa2f9dd7b51eb332feb4fa0fb746a28d6b28b4557dba048f2622d56a194b0992ee98e20e8c47933e3c1be6e7" },
                { "sr", "843be7269738dca9844feeee87514f5344584a00cdff551516ae346a3fd99b83f547524388379933e6c2cf51e324a5bd16c716ce33fa614736706f43de31b7d7" },
                { "sv-SE", "075ae0cdf138c790431336cad5a9e9ae0ca1eae712b0113f94cba252fc703860c0076819109840009ebe6425069afdbd71097bf8f59d17d40d123d828a8d2d19" },
                { "szl", "7a2f5616c77b6dfd086602740e19f9398f3e72d1558c5f3ceb29188bf7fb35251e2379f1a342f7fa90f313e7588c9a955b2aa44c8fd8b27b26785cda4eae4918" },
                { "ta", "0b000bfc351d3f39995c541beef1925cdf97857343a6eee8c482d88d4effe651a3b119027afb9b9373c22b4671761124c0b12f6a8f05d56a3262b3886b082b71" },
                { "te", "fd68e3c40236e2cf85fc0681a54abad6af5f4874b2bde377c9e433c8cf02c32896d4ee4ffcd0761655e458c3c313e778703f5a822141e2701b842891309996d1" },
                { "th", "ccbff5301612ee842870ff1ed385530742e74977ee20f63633cc717e6ccbea6c7f25aeda67e273d770b711fdc49a7238692d68ec69f9d25e11a22a537a1ea6b3" },
                { "tl", "b8d0ebc83dd3e1fc70a8c4ec670ed7d8992a4b22f0fa4ae8ebde343a23899f6ff984209efd97d13d6e94e045be109d135ec1f4656795d302f5e44675459dec8c" },
                { "tr", "31a456490ac1e8668096bb3edb8624060925d2cf49469159ebb4b3745e8785c4fcfa4682aac893a6933c38113e81649d2dbf08a432da9f758236133dff70a8e8" },
                { "trs", "ecdf01a95d62ce62cfb3f98495970daf30f959defa5b445ab95a09c641fba6498e07aab69cefe83c914c1bad6b0a0c9d5e6112b4f920cc10380c216545227f46" },
                { "uk", "6e7dab7e0e91c95041310fb2533e2df32403eea36787c45e67aff1320194e41531f99d75e6a3e1f5ad1a29777582a1e8d7c64abbe56adbeb2b843d2fe40206ba" },
                { "ur", "e1a3699f62bcc6fe107ae5c8a6219ff9f78ac483c41a92940ada9a988afa176e84a5faf6a20ed71177ddf8a9f65542be9552bc07567405e53625ef5f638f4096" },
                { "uz", "38a4d9766c5e19f53a4668e9ccbb45bdf775ebd764387a635600a55d2ce5a920332d2f322374323234866f864567ef8e18feb7feb0eff67661df7d6e5e1bb005" },
                { "vi", "8a6bea7052297a4f20088e432f2b782852e75f5f117b2a604573f7b093d892b0fe47079c887563fd5fafffa1ae7fa2b7c3dd36d6e784b97cd083727ab5fe7625" },
                { "xh", "1dd5dce3efa298fc978422f2cb72d41b6e32c6ae7e57bd9546089e433475afd52e1f4ce056253043a109a48bf675b8b8ec24e579a51483a32ad60d36bf035c14" },
                { "zh-CN", "dad0144ab38d36b5154d45461cc7a8e29dc413acc8706cce99d3f76774c4f16b6358df2b431a717fe0d720cf4231035d270b2baa69dc066e14969803784aede4" },
                { "zh-TW", "408395c41ed5f909fa44b5b3e62c0bf275a9a1e096f8bdd828342905e28b97f4dfb8525042bdbf58d496b3305365a4eda00e2771e3aaf3a2fb33f1b203a5d4e1" }
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
