﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/118.0/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "91e10456c90a2a64e92073c62966ce74a3c03f93a463696e0a3fb0e1156e6463d7b5cf4b890926f500d787253967104ff76808eaff8670eb66571cbcaf88ad80" },
                { "af", "5b69adb246f3ff2431b7c3ed00f67f0e054410c59ce863decbf5fcfecab7dd2c4cfddc62eade8b7dda1e72c965eadbe5c29b34723602f710af277b611c3f152d" },
                { "an", "c34948efb34d28287bddd75a36d1a11f865da74629e778b2cbd772f043e5de7a9262a7b3688f08fc28238dd0ed2db99a0831ef6478f6b172bf0eae75fff33594" },
                { "ar", "04e470747c204fefe96fb4e24e9c76305dc019c2ff3415289b223b6e5d7f1b609a02baf693ef65ef5ddd3802cd5c56b44c49067f2e5356b212d76a7babd2a7a9" },
                { "ast", "a73bb6f9b4f9153c563d280800ff9ccc5247451bc193e0752cab6404e2e2c2a376b62a080b5bd3848aaf098961a4d8d9cb7e501f11a9f8e57359eaea90c65c8b" },
                { "az", "feb11b5cb838612a57de075569f7e8c7a8c646bf1386e8bb2d9d4b2e1bad1a3e8a6f95c4c2be87241ef22267b46c2a0c449db64c3973fba54cb19c3736bcf856" },
                { "be", "0c7352be21325448d5176831858bc36f47104d275e3c7d887c21596d56347df778cd95c630de432361119cf3cc0ce4642f1157cdbcbfd866460c09ba60f0b58c" },
                { "bg", "0f1d102f6907635f3411e5927dac9bb132f080500a9fdf01f865db00012b7f3c68c4f21da22e82f728f1d1c07e0acbaced32ea7bc6decc0b5437288240e7f9f8" },
                { "bn", "7cbc20d4e5c9f023d9cd74e388ba4a6e3999ebc856e1f7be9f16bf56d37966ebf6cb4d64f34804b222d0fb22b8bd38750a26062d795173e1ff8239beb537c40a" },
                { "br", "e3b80424f60de57cd0667db995eba879a63e90e8f6542671cf369465a387a176936c8b474166f2ab5d6e937f85c07d217ddfaae247cbf0807eca1b2b478a5f52" },
                { "bs", "9eb966b9741f1483d6e6367bbfc2b64e4850e9ad80cc9c54485bb6666c6f0dc7c26517fa89c1961d771cc2743f16ef3b06c82e9733e5a38e38e15a2346bf8d01" },
                { "ca", "901271c2c9c19ce703ba01c90c6c5b089c1a873c8e1fd281d79117718a626253fb0bb1e09c16a851c904c2b6ddef959a9737435b4e27cd754a509f3adb45dfb9" },
                { "cak", "8e8ac76441d0a458a1e950cb28c99de843cfd68ea627f1f40c30c83f367464a19b34c4d8c5063e675bd294077e04d23a16b64fbd7b1bdfb9f68b95e8def0ac00" },
                { "cs", "6a89699d7e65e0397d9c386d3e7c49cf649a528e9371dd2d382875a82f13e2e68bfe2e2b981474a278ddd4a14051d0683eca38380058e7ed93e3018fa6fd7036" },
                { "cy", "9d49e4b96b723148e864dab74ff3464fe7e00ec6c38c8b7d76306c2b8001019b28361ea5b3ee38b40464adc2b2677b55727b387f56ea71d5b34132c105051f6c" },
                { "da", "fc30f46f9867d3b47ecbea2727c40697b1cba4b8a3315288285c1ac4081c88178700fffa1da9a36ce4c2e0853aecd55fc4f665a8a91cb18c15b629958dbaeae2" },
                { "de", "a427c9ee98ec9e14d7468b1f7412589992a1d9c0be4b9f67da2d5d5f2ba461f94b0ffadde44b73ffcba1020bc52fe7999e015e43899e54a463cada7621b6f1c1" },
                { "dsb", "b30ef989a69e1615c7484f1640a22d565613bcaeaed309825000898ddc11f87e3d8e9f92d5b6ccad99f08aace6746c772f874995d8a4bf7060deb55f3f8df121" },
                { "el", "b9e22d70b5f0380e72b902e7e8977be0bfcf59d83ce94dd07d668c71120101740c8e3e71f31da2a43313283ed46cea20e16e058e72e0ed7eae1e9978ace9aa56" },
                { "en-CA", "2aa379cf20be0b19d0f57874fdf2933c69896e106ffa14d9784e92bfc5f960834b365ee23bff8be5db2ee3ace6e3cbb7c601bfa22609712197d90855bacbf19c" },
                { "en-GB", "77664e9aa903a2a7f72735084e5680f35ba7f6305e4290dcf90d8ce3218390997c3fef3aa69b490a405c286a83c67a326c5b4755ecf315c4f1020c7ca4a55038" },
                { "en-US", "e6093a8688f2d169c8b0928bdbb6c10429a336def37d4535240be42b0ab79228677f9543e16b8a4dd233459e7d8dd568f121742340fbd982f09a0e222d94cefa" },
                { "eo", "39ca93a97b6e0488be7052dcd51ee89865ff35b1bd4200776e5b42ec43207ed62804ac81b085f44cedc7323cb230e06e0991ba8ee7c0a058a99d8598105cb049" },
                { "es-AR", "254b754593e682599b6173d27453e2bce893569669688aca706d33f2b12de816e025f93222e2c0cd258c995a338481aa07033798c84f817f3a460e74969385b1" },
                { "es-CL", "d45855e1987de4aa13fb40c2300cabe91c9e3355e98b1fd2ee14fa5e80521372c5381d76080eab9893cdaed929427194711f6d543e160d7e34696190e48cd425" },
                { "es-ES", "0579421ecf330805db6fc1f4ec51df260c3056752ad611950d798f20571845569d52ede6f2013775e528a5cbb0729ea09eb3e54221fc6f4bcd7eb41b26715f62" },
                { "es-MX", "a270cd17476a69764c97e6702693dde0b9aa248f0f02ca5e0f4dd448ab2b11e2716253bc1540c9cfe790d3ce50bdc957db8a7db23a9cdd0968d5601eedcd29b2" },
                { "et", "01f02dae336ceabd33929b869a34326546d0a27ca43d446a2ca857f2043e978fa45140f5b3460ac78cf335d2c12c0e269590b6286785146a0803e379594500d1" },
                { "eu", "f0a0a5f44948a7c27ce6580636d89300a2ba3444e204db413ee5959c0a893853f667609ecd1b582b15c59697974aa45afa8dc2a0ea6fde4217955fce5e05be1b" },
                { "fa", "b5a12577baeace78c79f1949626e1d2e47ade9a1ba28cd6d22d874aa8ddec110978072f11089669f92818aea22bf3617dd6ad84da0a60637e6c5e2c297a876ce" },
                { "ff", "47db5f6db508ff0dbbeed88f6804eb8b4f2ad9c1f9de2fedd01c36d22a7fbcd85aac8999346195904d7a92c0b9db22f692037f13716f18e1e1362b35c2b3257e" },
                { "fi", "c3beeeffc4d1c4ef55fca70b825f72dbf2e0a560f9586166510cdef60891c4cae52b4e55e5f0ebcf3c7a9118ad99e1b7c2275f3131a9fa76fe432d60ab51f44a" },
                { "fr", "497c111b650662c360c871586cef8b2d5f199a33bcc493905c3c68715e12274ee9c7c6a327ffdfa4879c75aab0a4c244dc97972c9f16e856e7d650d13feed5e6" },
                { "fur", "95513a094c1cc1bd15664d5e386db99e1a3123049cec7338a8875c3d7d16fb0ea0b425ace4e718ba5e25524b0922430dfca615519e26f256a2a597883bf3a5d4" },
                { "fy-NL", "810c57429a5c823d74de2bee1206338b8b1e8547c09bee14a59d998ee072a31c13cef0b8ae40f56cb427ccebb7ad36745d1b20d7fb624077cc5100116f981690" },
                { "ga-IE", "be5ff7521fd204b02d6a4cca87f84bd146eeb00e3e02e4a3e26023b955d6d2942f536d17a0dccbabc1148fe49f6c6e2ea4f44e790e503f203214c054b2578408" },
                { "gd", "5c7e9be11d8e9f7b7ab77af4ce34b55b79f62189c2a28df53214c857c9c070f9f8825468175272005302f4e9fccefe5f0fab2042135ac79fdce4a99e999fb22b" },
                { "gl", "0e5acdd01de5cd166bfef6eef56fc68d4f51a45965faa039014765cfeb367ac1e657b2e4c87933ba2646900b6acbb163f277bd0400180c46b9b1f7f7dc3320c7" },
                { "gn", "6d557a00b6ec99298d6680ed88ad10029b1f729d939ce76600930ce2775ac6c67da0d8951afd356e6cb361997c8bcb792c9afd5382fc753c40be8e7e87045a08" },
                { "gu-IN", "c58f073f87cc165e36703a4b9468133f74f21ed122cf1aafe4362895a05126dbb08245c2496aa6b7338b763e4972ea2171ad949f2585fa411b7272b69bd00565" },
                { "he", "ebc8f2fe4bb242ea25c8f4c8e91f695c8f7af3a51157d6ed90ff68ef0b75a86f01431420d53fd364f0bd0442c1eb7a60baa076e1c241439db6e4760136a9f1b2" },
                { "hi-IN", "463ca2ef716912fe0c29e9c0bbcc6dd90ab35cdb1fa0b295ae8aa6d65da119b8ad0922e56594d77b3b516f700a668e5d30dfe73fb6d0cf50b3043023f5727925" },
                { "hr", "0abe2e6e111bfcbf70e355bbf18560aebed08ab9ee40aba6f88c67678c86e15f0b4b95db1a7a744383b116188787af048831ae2f21ae32af1a828634cdfb33a4" },
                { "hsb", "a6f312c55845013a4f857cc227163a3df4087ebba3f5cada5b22cd2bc1ec79f5a9bc77961c3a273d37d080326a82e7da3bde8f349c792b33d61c3dcf4f814552" },
                { "hu", "afd69d632c7b6c59698422be4a259466085b9b3bf88fac2273264e6fe3ceaf1bd344a04b8e2e9437e3964905fc8317faa2095be6df16f73760d2838c632ba04e" },
                { "hy-AM", "7f252744557f6140e7f3b091953ebe906b69004e17e2cded00ef644838729d20a354a99b1247d2abc979268d9081f533b4eac84383b0e24c701ced89731109d2" },
                { "ia", "81ad388ecfc605f773b79447aa25cb699a409e6b65f54ab941895b33c1fe58df360e355ff27da7cf17b9d32dc3a5f21923eccedef5c0ac5f469c30998830c10f" },
                { "id", "b25c992b483e7ac942250485fe0cae3c280c0f37a9a6e07c537cc88724305ca12f6aaeea14ac87919e5e70170d2110938f2126d7952602dea290bcdbb9d937df" },
                { "is", "dfa02732319654b6c46ff180269f2fb3f08f4804a6ffe4e5ba9b0e199131ff471b2cfee17168327c24b9457cdfe4c8140e21fdca4f0a7fcef0999641b42aba1b" },
                { "it", "d48097d85f6d5ba99132bef805f69beb427356d4fea83cf83f8c35c2e88ded764fd8acb08188048ec8cbaf4c8bf266cf90fe81c92e8a01367d9cd1c2a1a59f68" },
                { "ja", "1c826b8e4db929c5f5eb759852e22ed887232e1ef54725178d1e9b6c04b762cd4557e64f198735d05af54c8d838c755e3e898c807277c818cfd47a081abcf2d9" },
                { "ka", "83239702366553f24ac767ffaefc2a6c3587fcdfd53d5ba81b1fcc5d38215a9dc3400e640f9f8ee48e6664f5fda88f033a046cb7dcbf38cfba9a95f71e5f5f1a" },
                { "kab", "2a5095293377ed42d86874d4225f92f3a814f663b218265eda133c85d18bd60c02d829b007f1fdf798582c217970ec1b924a17ed079a9f9a5d355d9fbfb3ead3" },
                { "kk", "b6dd77a0bf76b39d0755e652efbe86bcd6f8839f1ea1e5628d0dc5ba6ad0f1e2190cdda3376a81b94791f9a85def0ca06f6f31a40977dfd66f413d992c306acf" },
                { "km", "f88dd533bd94519a9814ed74aae20fa19b9d6734b9a94d958c22cac8995b1f746089b9773a8e9303543dfdf8306eb44386051079f4d740ae2b1a5bcaeb091cf6" },
                { "kn", "2f798ad6b530450b402fe53b79e91bb430f00424a3e091ac982af967389867ddba64137840076f725642e29fe24f1bb7178023b66e6a61240ce57eb0bf0aae75" },
                { "ko", "4e5d689038a1d2bc128fe10b5f2870b0cbc9faeed3970d25aeeb11852578a94116810363ff8bedf2686657965a044d0412b2841b4ab9979385a51812cfe943a7" },
                { "lij", "9b569d5a1050e3c87fba37c8a69b7af394df3933eec77b76415e30460ee50654267402de3b176b44d574a3d5fd8af4d584e2119f580aa63ecfb063f41513413d" },
                { "lt", "0764f00c80a25c30530709e16a9775ba852d7b8202b93c13e03ba19617d29230cb315e593ce219ad2a98ee3e0360cc6a43466627436eca7a92ae9bddc7cf84e8" },
                { "lv", "c6adae434de2aaf2ba4bbbb73646d3dbecdd16175ac1b97d65a3a20832843ea8d5d45bdebf2e06258f3d3f04bce4e873224d980a04e232c60b53b7df141c3825" },
                { "mk", "1091e5dcc605deba68057786ea8a01be45883e0f62dfc5f436bfd149afae394e5634244653850f0225c5cbf610fc964d4141988f18e8870336545528b15b2879" },
                { "mr", "8a30edd6ea4ecde04e7cf153f919e537373e2223e86cae367b6201052006b9216e205f272dd673982622c41dc3e58c6f5da3fc2447a3de731a4073042c0485c6" },
                { "ms", "e10a2c03ce905956d693d9f9aa967868af5e55c0e2c731433015fae0de0f64b260f6ad2d69db3d4489491431fa7e02ff8b0f43429a67fcaa3a9fd793053febee" },
                { "my", "622c59c63834a0e1d3da95aa5b3077e6de851bcf177b067928c1073cea99c0e6dd4591683b0377a042adeb06befb7b84cb579bf4672dfc6476ee60002c9e8683" },
                { "nb-NO", "559cbe376b86c3e46ddba1f31315779dc85bad8c054dfe2eebba20e6f9ffae623db4c824ed0bcacab8a45e4989be9ed7bda77c5bdbdc22bfbf249a78e8a5dc2c" },
                { "ne-NP", "9e76aa66741f8f7ed173a8f311488a1fe830b51d589081b5c749ebedcba225adba0e2a4b1b3cdd65014e2b26e3103534696d955b80f2fc1ef9b0a15e2358880e" },
                { "nl", "63f9f1ac95b42ffbc5916d6e96d5b1eaf09411ec00202017f3a79e7146d7dcd93426dfe5cefeb9eb95f85d93a20fa0fd7d320ff3b40185d32e6c3ce5cd15f2e2" },
                { "nn-NO", "c1b12626c76cd4dc8700aff31c26520232a2e90908e9dbdd5f59ee455e9dead0b8e79d5f3db1242d8d341ee6a80700b9e96a5767131e95d39811ded0451b00e9" },
                { "oc", "130b52048c10bedd6a761a747cd8a5405ee7ae7f8a98a577c58ae5aa510f7a6e2fc5b6b0b0271341b3d1cea322f2012daa0e6b9d0e34081e0facf7633dc27c77" },
                { "pa-IN", "34a4c0bea6dc99a65c1f9c8ca5f452304c9a09d37e424107b211af9637151b9b70934063937695df20394e0cabc89a70b5b93fe5db83fc449ea9df37282f50c2" },
                { "pl", "5883ddb1b7af10e5a51904d74fe429ab29c109192dd83a78f5267a17cd4c373c6edbed35346fec22939b79d227c901729377520c427be66f885e24176d10b1aa" },
                { "pt-BR", "f11baf990c9085439b9e9227b6e049beebf22c326317a97fd64cb5b149704b731dd43d217108746de92704e5a5fc4f90ff9abbffa0398ad0074df5b901f2ab21" },
                { "pt-PT", "9ba193a59e72e188efe07c9c6c3658404a5699dc04d42d84b58f454bcc3207cfc5600cc5671515b5ee6c31e60594df2873c3b26add6379788f1ca4173352c6b4" },
                { "rm", "1782ebf529e1fa65f56d90ee42e35468220a643d9b3aeb79006a4e82b620556890744c09b3f170ae4811361d6b1c404d06ca6e4aff33b27126a5f41e5fbe249f" },
                { "ro", "92bb8627340538a715078c69a7aabac72e895d2db0bea88ccf56fe4e8abc28dde95bd59706d668596b68f981e39f41b0331b8e0e587693c823c158fc5891cbeb" },
                { "ru", "f7d133a1d1a50b93c9f63e6a679e23dc8bd2fda532996f8fde4f812d959f29b47c12d5ec780a18ddf3b169cac3fcfb2d2b27b68966b1905d0830947e30f00155" },
                { "sc", "559f20a6b03dc2df71f86095d8018ef92cc09de218bcc12ce9c111daa1ae34633d92eea9dee49ef35783f6df6bc62de80e85322e8e1b7954e139a03cb04a8e20" },
                { "sco", "bedb990177ac0ad487bb634798feeeb45d86e6fe4b988e5f3c525ebc0123b2ab271272d78e3f80073978041fe3c8266a156885fabdffe4e9fab5011b2e055e21" },
                { "si", "483aea57df6f933b6cefcd720e3ce09d5082930ca1b312d4f15f2f6dde9eb0ffd285e28b0e79d27c628771a9325fc75b1a4e52f1524eec2da5419b8357142f14" },
                { "sk", "61517349aac645f099400081500966100afc630fb167d8442d59af30c515405d3c29bc96a587b029a083dca651984b73ab2cfd0a2da1447a1d86d580b9ff78f7" },
                { "sl", "40dcf57d8f0dc4216550056e4cc6dff5bed2f128b709d8af9eb835cc22395469e2e7d5d0f2465cc86611c6c9eb974cb3eb090514d5b9635ff15a41aeb04e153d" },
                { "son", "ccc1fb9710974b20605f3596f7a5232f84baf418d47245237f0cf87c39beea9fb1d109d4e35cef4714e6a3e4a003d779e7bed4c28b563d14662bda1c461c04ac" },
                { "sq", "6e07618a5c0297c2ec6230083f155cb93e6081c1595e4e426ff1292d7322d84d94060e156d0d334ec9dccaf4c9b550116b0eb41b019de337b798b454336cd470" },
                { "sr", "49307eb7edb9b5cf1b698ea497518927e3f148f38bdac7c2ae7791178bb88930eb2766bdba183ca2c795f4fd1e3080277f94f2c07874149b7e6d720bb7d0dd98" },
                { "sv-SE", "c0563d72db81f19f28c3a775ebfe2670b6f86c9f7c9a056bd052eb55008ebb040ad9fc3124dc746cd2d8423e1bb1f0dc2622e7013e50ed80861c2117f4b96615" },
                { "szl", "e712c69292b13c75335c84fbee8bcf226722325080ae9b6ad05caa1d62d6ba50f1cdf4830c44e5f927f33409c087f9709d1ebf56d1191a0875b17fb6906e8149" },
                { "ta", "7442487c8c9bbd6688c017430c81fc802ee0b9554cda9404c1c837016837eb6b89164fde5d351c884dd8c41564dc01f083b746bf5caac248e0ef5ba254032490" },
                { "te", "376ce13fd4eef0ab8068f4978b53110866b97d9330592c4aa44711ea8e3ab455306c8d26baf28910f456825e357553815f1fdde03644ab5e2cdeb05d0c211833" },
                { "tg", "866968140f223dc7da3f9eed627834f15eca02210eadfa00fa27602a49d9dd2ef68201dbc0a469444ea4faac6398e426dcf083e912f7e388d5aa5c2de5d22b26" },
                { "th", "da9b9992ec1d55c7668b4b45909c61959d25d71b49ce109a0f57737e68122f4656831e285db9835d7a6e1cdd76a0652c109a917f29d5e54568952aef9d623903" },
                { "tl", "d671ecadd855c8b95073988b50a61a972c31c010bdfa6b5c61a91ebe41057d14666ddf90687df2585d628904ae956553737984be37a523c41dbca14ffb9191e3" },
                { "tr", "069e746111c11d269bdf0111b7d3743947622c6114ec6e21a64f70631056a5418ee2e35c7f191187b15393edef8f536779b54695bfcca52ce19ed21483da5d23" },
                { "trs", "98e0ae74b7c2e99112fbceb8274bf3f95e4d7782a8e509484fbd4a3485e5de375782f8fb8b0bb45f68b4cb4a83b04ea05d64cf4a9a5473020028a98f258803a6" },
                { "uk", "b14fd4aff9151724deadcb52225f7f8435163679696929f53f98b755794d1650bd643a52200db96cd46400eb4e9d4124712e32eabed9a970b742c66032cb690c" },
                { "ur", "a592c41d176d5581149a6631942c6617e190584b938e074285c3910b0ed792dfa5f422ac878634cc503426a70c57eb8ed445736e60430e2244d8636ec47e8d1d" },
                { "uz", "4e80a484cd7068eb94950f9ba6aa518d80b28810425acc2fc28eb8a85993d33e882879845ec0862d2563c3dab1cd277ec3d81ca2370e40076785a7b8d603fa60" },
                { "vi", "d91ad17e260883d9d456da9adf682b9e1c6f0ab4753901bd52f4090756f71634fbf08f96fb0cc02fe3161624dff3af371b1a7c9f99708717f2e3b9e30af67d2e" },
                { "xh", "1e41e0dbcc158edecf361216947b122a047024fd12ff8a6239917a6a15c801652ee8e20618869d5d8fecdd6b2e343207ca749a758b3a0147f655ed0bcc9f543c" },
                { "zh-CN", "9dcce3ef1fd59e8f2717df7199569f196424d3461caf74afabcc50debcd8ee583e1a991ed800cf1660bd931de0bf94ea45b000967a267227ba05f490525fb9c4" },
                { "zh-TW", "802b0e27a46f9727c1e62033c6747d359599ac35801ec3e7b5d72a7ea1e8cac510af7d8d609b8f851b2e54f13eec5579a31fd95da83d4e0196e727329f8ac617" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/118.0/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "2baee0d3a9b01abeeef500422ebcbe752e730be108b159266c0f67e97fb8e864579a0a57498cf6795c785d531443feedb441773aa239d8e57a629475e8228906" },
                { "af", "1b25f57dacd12c5ae10d4068fd74ac66c9c1733c7b9a2b0a1a1f8e5761f19b4a1fe013e832ffa00c64f6ee6e7900d739b95ebf60d7229fbddb8b7ab4650cdedf" },
                { "an", "b5490ad7514ea36e26fab461f2c7a41a5aea725cee882fd79d2c1f874d00e8bfc4c4f90360c3caf4248a3925c040bc186f19e13120672af5224c64136652fff7" },
                { "ar", "04b7bdeba85246917def486e9da535ec408594cd117b693b8b990178548ef84f9e003c46df6f68a45f5343c1ee3e685bad40ccc932635bc55b0c03abe9b01202" },
                { "ast", "d750e8a8faecd16a3e59fb5f6178a621f31c184a8d02b10ecfd9d0b66d46ab648dbe6c8bba13c6b424048e24612751dc629456ad335adc6a28ef6b4e0b6188ed" },
                { "az", "9b971f2895bbf59bfb6048491db6f93828b3deedb8ecdd76df96f5ba0118ef2635d3d534d95a7902c5b7bb304839ba010c051de4584f4c8067cda4038a1e9b1a" },
                { "be", "2478303d323f4cce19280f63618144364f2d9aff1b67cec472fff8dbf564700c1195db78a6d98bd31baef5e3dbb44a1f2fa89b9513cfc7defcce55ca650c5587" },
                { "bg", "d7123937aa3a9facf40da916248e5e0b2bf4f89ed7c932df0a729b69e9e01f76e9d256f86c1898742e5eb980e60adcdc9021e64cd9d46b7eec100afdf1b684f5" },
                { "bn", "bac105a5567b4067b4669c4bc569141b423f55e7486464937c03694e89e89c006f7f152d9ce636ddd474783df2d6678273a4594a5e4475583107686bc149c517" },
                { "br", "d4725c306d4e46bac3245701d3f0871a75dbaa7593ed3fbcb312d6286956462cfbb2983a6563e2f1d97d1787cb53e51c0c17e27c4c4b14952dfff648a3028879" },
                { "bs", "d3fb1fcebff2600de5b5b2e46dfa23815f601b20f91db853fbb130e163aa20984951ec83cffb44252c7ea5ee1354e6c95075a38460ef419b99405c038e777ef3" },
                { "ca", "bd34217157ea429bb4e91c381d537516afb060d5223a4793423ecd0c87830a24c5eb6fea0cc9cb61f1deaf3ea3fa2314435ad7b370a45e5169b94510c45a3b2a" },
                { "cak", "ea79b9f2ce9c609829c762fe3e60bfaee4b04921503b82c8dca09491bb2c11c7d36a9fcfa49b08ad52b3241bf9741c051a7f4471b30d795c19a31a5fe186c77d" },
                { "cs", "853f9e6721f056f4abfdd9311e25c1474ede37d262bb57e4cc7c31b995c6ae97f2a8096f0000b7b3992077c7bd391e8a2d536edfb5f2532daed3b870ebed79e2" },
                { "cy", "2a67d56819b8f5318ec938c1b9301c1392d05a229d06b5fed07730d2012e43d86ec6305c377a796b75e3b8250672bcaa2626d652a205daada9687c72420dc768" },
                { "da", "000b6ef1c426a71e33e683ea7b06a98b2fc477b8d88f0424e9813782e1225c6df0b996052d388dfdabf58f57b7d1d80750acfb3f7604e636f1bc7305ae362ff2" },
                { "de", "f88346bc499714f66975f106f8983dae429a87e8a016de2c34dfadd61920d9fb0cce2a7448bdf52688b8ea868b989b950ed1739dd21b4d58c30589519249357c" },
                { "dsb", "519da23576be5ca763131803e157a9657f79babc958b67a38213b88c1af182deb5fe20ec66b30b2420da29d220ca1b31e2e0ba746112c3058b7b4dcbe08ee76b" },
                { "el", "2d066fba0a501f1f755cd70bed573dfbc5a84244d5f53f9bf71d8fb316f20ebfb4014df2c69ddfbe6952c92a29499bf51ee54bff0c3134cfd5f6b9fe055db650" },
                { "en-CA", "076def12eca2ded169da7a59f281a6c8fe79b50845051759c2cacda2897d148a737c4277ae98318e58205f34c5663bb13203349bd9ee1c093cb9c5ab06da52ee" },
                { "en-GB", "f1414a2f894a40255d6879dd66cb9227f809c89de41bc90e444030df09a64b41cff3b2450beb8fefbabe4d3dba7e2e2c4562937b88014a337d49fdd84b55a2ac" },
                { "en-US", "f0d652dec4a4c9d8fe1311a9eea0957f4fc3c13bf56ff4cfccae6f29b3ad67629bb34f35e65c96c48609955c5a716b2ff80a2eb4fac5e7ac84af288e44bdf412" },
                { "eo", "8b8e18f0d4d7a56b4d15253d46ef4b58e6e3055591331d875aabe824a55187edd022913bfe7b88297de345dac8d31523313db02600730829d5d3c01e67bee6f6" },
                { "es-AR", "3a1773c9d80efc46e747759a9de3aa746e02379397759a3342f844194ddd2982d693c65f32bc5fbf70a4122a714da4c252acac6120c5241e9e6352a3ee576e16" },
                { "es-CL", "29bd8726fb84aa2e675c3fa4e3515ce11fd75e1a951e1748c68716dcc6d6cab581ef56bc5de669d4f4b6a7f3a78b072caf0b52bf8c0353f7b5b79b4acbc555c4" },
                { "es-ES", "26bb233c7d2c7cf19818b65b52777cff83caffabcea36f8223808c44d3107f4d02d16348cc89ae3208990073618cc5a869ff816f7f850a05799299ce13cdb768" },
                { "es-MX", "de9f1d2ed73e043bca7855ea3cf83893ac04249687555a40aaf585c68d64abf4a3b9bcfb5b9515049003f735e3a4171a1fdac9e0f555675331b1a59cbe10aecb" },
                { "et", "d70aad585a54d7bcc42e849429372c726dc22cee21f3cb929af46c81e46ee0c6d99e6a79fa4fea9bf2d43974e9a8d1a6a9251dd78da189f38c21c322ebd78488" },
                { "eu", "5932d70fd8a5d0b3cf9d491542c866fb6851f823b00ba4448af0732abc7f01ad90acf7b3814a88a1379daa4d57dcb0f46fd8d362e953fd0f557e4c9f07ffcdc7" },
                { "fa", "f1d3dc0585016a717d26ad3a7fbd3d0dfe2eadbbeb35040c584c690b9f0dd4e6b78ae3ed58ebd8288399ae7cf0391704121dd16a6e2a42f258178026a94cd112" },
                { "ff", "3e188cc4ccf2b0f262f6aa2a6930447eb3d724def3abfd87dce7d22ec325e0b2c34377516c2b147d0cea7b321f2ffcfc48f9dc02e7905965d8a64d263046a080" },
                { "fi", "bddd74613bb1f1d8eb1eef6ba65c5f42bd1b6fe4c6e3ba9ef6cd1e9c8da2c864cecb3e95871be8ff3e8f1bfd38ccdffffb12cfa1b3781f6208b012d8f3df2082" },
                { "fr", "be2951d533ae6659d430bf8bdb19f115e3d9e6d9845b5465d955fb253e39d162843edf7764dd6b7c4abc9b5c34c8a282096ac538f716e5a7e1b66eb4c3391cac" },
                { "fur", "2f7d43a3b4e663580c59cfd42ae16957052f88fc8f8347660b08926818f19fb3e3398dce006125f3b951e0a1aea8738827de50f862e92f4459028ed493d2fb5c" },
                { "fy-NL", "e13759f0d8e56ff1ea3bfacd24aeefee9e6a0e6aab97ea5d757c2cf7cd2bb8cf578d8686adbbd8867d071454ad78e9990f0efaa55848a642f6785df211084c97" },
                { "ga-IE", "8e56c0f8935e794e67d187a5583bef896f6a58e9ff8437498624eafee9883cf631c3505e4eb3beb17b6c360f8bc6c5449de307ab98f6ecb78bd72fa12008fc92" },
                { "gd", "9728b5aea3b20615cd59fd00c4ddedd36da90666c65baf38ca8cc28bcb8bc13f6dcc4b3a09dc8266785f0ee785542dfaca2b2fd16693429344b081e06ced6d6c" },
                { "gl", "738344ac0597260aa81dfee7d30508e693de5914720030c06f218abdac36d698c8961283897891f1afb451722e86b4cf8157877d2d1536933f063caa4d969ed7" },
                { "gn", "2edc737078cf9cb0c0b1d1f46c5496ff4dab6eb45c66b1dba2b9a01ab1b3326ed709d35a5eb3f2f74700eb323203b420eac93e7bbe384ca77e7ba7da7441e539" },
                { "gu-IN", "b8c40e44c7f41b0e3ab9ef306cab613bc3be888b235f1ebb30dd2d590560ca0b0bb959f0b6b02f1786536f15d58ee7dd55de4d934d794e5c3f8550f551718293" },
                { "he", "eb8689d01354fd023aa47e15717843912a20211bbe998f293ed1bb1ce7fe1837f02dc1714ecf0268ad7f8680579c58ae1fe83352ad766a768841f652d6a6a6d2" },
                { "hi-IN", "db066e6743209e11ce7719454b5a2967acb155e767cd0be3867e700fc97ed3139b5be450225e5b5f0e5b31be9fa6cd57c7d78e8422f11f5dae131a8fe24d8247" },
                { "hr", "87a536563a025249940bf69d5b1df71e7e64f79e4b4dda28aebeffb9503f269e3329ed0f6a4769abc2ebd3d861b1e39eec23375b298ede639dcf3916fc0a4ce6" },
                { "hsb", "cc39c6c46fd5e0d7db9b8c15657753b6dbc8f2676ca4871e2f189d2694dbbbdd6a336d90c1a13dfdc7f2dd03160a2db3ef885cf33623142b5787eaa967cb81a2" },
                { "hu", "34ea5553eecf9526cf150e6a0788d8f352afeada1bd75296ce7a365d586485f25a8c5caa2eef81d735ae147b92ed97d867b03e6c803db5d5dc226c91b484c0dc" },
                { "hy-AM", "6f8b812ccaa34e831e3a185d1107dd88eea41ed18e47bb3d06396fb487c32ce16033922dcb67c0f3fb3c2b6cbb9e8ad48b9b60d5cd2b17bd4d725437ea6889cb" },
                { "ia", "69de2849229f27a3d25fa59424717de6452bb540fae2af22989d30ad2d856fff2709d3075d4b317806140ce2cb40558477890bf5697d32701cca2dbaa7442290" },
                { "id", "ebf84ccda4779b33423daaa516a2bdd253cf81ebaa416c9ec7591e95ecf2a392293cbbafe23f3b28f8e6f2f9d58b33953200b161a4d0688a3a966706b7d794e4" },
                { "is", "95559b8ad72dc687b58465fd08d8ae092a4d115536be8aeb1bc75ce9bb9e8cc9c0f2ed6722e18bebbf5250fede5f45e59639aba423df56ea3669cd4b638709c1" },
                { "it", "e762fcf625f3a92097bc4f518128ab5ff1e27af5896e5073645ed46f58ffc5d30b0f3d7b7c563a8dc04b09dca5d0d739e311e2c1ab83801ff8d9f332a1d4eccd" },
                { "ja", "a757c54037d4f461655a9601c5a4e1fa4ecdfe5be9b530eae9ef3d491b5ac022e32d54474d1d3049e991e8ae53640d51566e8c534086edb1d08ae0633befc18d" },
                { "ka", "40816fb7b5d368bc9d076bc17722dd9f7df3a7ec26c283406bcde75fc8478e963a461d05e1ca24ce8f5f8b43769e868c45e172f4febde9ec04a825b2f081c6a2" },
                { "kab", "72016bada4e8bcc9a8d5bc28e16903ecdc01477a2400b49711185deac9de495d7de629749dd98cd14b4ec6d3a4a3a9daf0f1016e9f31b3fd8a7faf3e2c3c3a05" },
                { "kk", "e834033abd6cb06c5c3ae15406bcba67ae8532b287f08e802377a54c3f12b3424b625960677a96b9e9e1c8698e483463613f8ced42f9bb3cbd34306a018362be" },
                { "km", "5a4bf51598ffae0fe8a883b9f64947c360f49f9e87e4e47e02e0c1d0c55275712ac9b63f12b4f8e1c9573e51fdfa6c299cedc873131df0f1c60f1cdb1e377f51" },
                { "kn", "c37914d930f74c0abb2dde71c1d12baf466b86d40e06fe82d7d0a58458d78b4ccbcadaad46ecef864645d43fa0169681f43c117fc49058e2c9f6b81ea25a4d4b" },
                { "ko", "e2e2c76e95aad02605dc22e25e7b78265ed6fe1cb4f2420689d4703ec1c6005319701c6729b1cafb6726980b715986de670c0591d117c6598575abe33c470c97" },
                { "lij", "0d3db92ae6dc83d4ad137328f33d23c2392738368b95a2c5fd2e545846f2d14b7fe0d57292b79ae46ea94f4d4ef764d4d200ddf5ff3d0f663bd94fa2c051676c" },
                { "lt", "c5041dd2714e9cc956a2169b0db58e7fa50af7ed2bf94d072f9a353c72907d8a9e11d56dabf669eec69a8f29425dea148c5ff29a7c798936d7638c28d3c6cfd9" },
                { "lv", "3f3c75b01b9566b412ffda11ae43e6f55a32637dcb2a1cf34f4aeb43c003e6bdb20587a2e860802d2614f6e3aa5c7369a33367b5a0a7e17a2b1b75067a359e08" },
                { "mk", "aa4062b49bb82f085593f321e89031c908d79492f9d239ff73ba2056d83186919abb32e2222bbe49c0bdb5cbcb87b2fca1cdec9630db600321b03d700ee06f26" },
                { "mr", "9c6900c86e83cedb55959d266020996ffc48dc58c91f2c07268730779940fa85c2956c211147c578e81782b454b5ae0a488a57a306258307543c87a677808621" },
                { "ms", "32194658fd2d2eaf78eb43f49a80013104ce6bc7fb50aff08e9670dc1fbfe0c291aef4f975bfe5646db4bf18efbe604ecfe36bf1f660be284ae673795e5548b3" },
                { "my", "d6c40a958703abf233ca8280d701bcde305fe36f341c069869bbbf5579e70752448cc20eb73430c52aa5a33f7f15438414cc08f94f562a048de4f6af8937a09a" },
                { "nb-NO", "e65a9e0f90bb0acfc601c6696bf61b330b51ba9c5da19095d152afeb78980ea5498231002a51433d88a810a0d1a0085c6c93308dcf00773698dbcef246d269be" },
                { "ne-NP", "0067406aaf0f341936f3392e5cb635b92ca30887a7fd0991dbe50b43d1b19121efbf6fcd14cffc41168a83308a8e156eaa36813e41bb45fde6a66512a855aa96" },
                { "nl", "ce4ae8bab6ba175eac8c505fc44ec386c3f4c34ab0bf09e43c0f560d3cee2b2d2fa701d22bb6b192ce3310bb68f91ec2863bf3ca4d6b1bf7675d3c77ee942af4" },
                { "nn-NO", "b676a207e79b266c9509316df1ac81f5a755e321dec7c4e71629ad4dbb5b788bb0b271d223f9ccd6c37c150760bbd203fc89dd60278fdf8cf379913076a82bf8" },
                { "oc", "292bd6ee6846f7431e88068b8bac9bafc3824dee37d93141562320bbedf8c5bcfe2a96b96ee142bed678471215445d7b0eca083c377c14c981a55d1ae3f21894" },
                { "pa-IN", "778480e9f537f940d9d2fe3b6512b012d9fbb145b5beb2afcc57eebaf9eca4999814daa770983aa2bc658b2cdf564726f32a51ecd5b04ad526df9ede4fbf3269" },
                { "pl", "744d1e012c6ddc47b2241bf664f8543dddd41d25ed0aa5e764e2e417be188f3910e106805d57ca7e4a3b2d11b4bdea6af7c37f065c16110a5d198472deaf9e0e" },
                { "pt-BR", "d93e34b3cafc22e2103d1ccb92c1eee69b24f52af845dcc82358b578cadcd8310335600d3df60e4b7de09d372967834b8cb52b182d1ec1bb0b03775abb1e3bfe" },
                { "pt-PT", "810120e799aad7c51a8bed623dabc02dc860ee1c8cc2f7eef333e0dc5dcf3ebd64391495f6bb7d2e871d536d7843112d9e8e77992b2017a44e27803343f9974b" },
                { "rm", "b4e2074b799470eb80cec1526ca03f178348e15dcdf46da30b3e3d369fb0bbf752d38c7786d93aef37299255e0c1316099b4cc5b24afb11d9648e81a0a05d58a" },
                { "ro", "1330f1ff78baabfb65ee1fce3ef27db4c99c760e8f276b0ecd361c07705df817f94b3376863ba63c68764272b74107736dba296c7d6c455906fc00e42c045c63" },
                { "ru", "dcc94519876d5ea9d333452c7f4264dce38314cb6b20ad342182044c73926124e5cf371e26cd4d58d48d1c25953750b5039443f23320791e82a0285cfcc748be" },
                { "sc", "b5bb8d27bb61c2a70a1d94cce9eee220bfe671edd7d2fa96ff4d5c85a1864ee520bddd5fe3da3a40c944609bd1ee434c28c197f4ab2c632adf5dfdbc90a5f3a1" },
                { "sco", "7ca5f5ac926c5746f048f29ce22471328f7423e4cf1e35ade3cdda2eeeb1f3a37b58d596874a13210cb8fa665d0b992d9a5b2b7392a90202431596758c9d08b1" },
                { "si", "c3d0b64448489024c3a6085241e07386aa0e2293c862adfcb46aa79365ef9f80c267b29256be842bf8b39ce3475fffe3776d44a5765552b392c87150f3b84498" },
                { "sk", "65287c3eb28c50d64e78dbfdb534a7348307d89f51594f0d290fb84e445c03afade96c8fb5d310602667978373ea6cc9595d45ceb7fbc0d8e2a4909ba69cad29" },
                { "sl", "598d2897fa691555f901c0a39e6cdca0fdee78dd849f234f5e1fc88f56b4546fadf9c9fa32e654384fa7ba16bf2291ac725ee44d9ad40b8347e664760f4066fa" },
                { "son", "72a0f0aa9fbe572ca142e19ef0eea150c8039d7c0a0834209d8ee30308124308218e91bb32fc92a0f374c804b458be08acb2d43267c783a52140e33785e0ef4b" },
                { "sq", "0baa60c898d6bfbc10edc46e7a4bf9c6c924d2f2c78a22077c349c01cc22e4d0f65b468d3fc285cf399ba5f6a054511262400c0a7e9aee0da3a17d26f39af231" },
                { "sr", "5cb6b32d23d7fe22262f54024d4d49b9416c1177737b9a4c4e6f83e4a9a8c094804662808dd9edfb679e9fae003ce42149e1971cad31c04023447acc05ce32c8" },
                { "sv-SE", "e627c662c0c3713cebd1f5b51b93b69e6c2f33ef7950ef23fab3e17added6af970f8099ccf2c97edd4a8af75e28458513b9eee26488643f2080b2e5696334e37" },
                { "szl", "386638fa02525dac7fe3147ecae838c477d9f39f9019564f163fd7ad997be7300c9e8171f5b441287ac7ccba7e66ee2606624294694286a114c8e21ec4e36271" },
                { "ta", "75e01d8f64e80125546c52763c419b7b24f83b4a879b34e63b543ed9958699c54739a39ea39c3c2e247475a4fc8e067b1cbddb510d82a90c2469dcbfc176c371" },
                { "te", "1562bfda5dfd3c50a46350079b73d446f89f90751b0df41647fd1732a114f5d37cbc04586dbe6272c38acf027119b44cfe371b08c26fae2478209244de50d8b0" },
                { "tg", "74ea5cbacb703fc74ffc6883a9cc7b696361eecf2e30d17df19bb8ae4b9dfad027bee61c24cde2d86576ef10ac1b781fe65f86b07dbfa216923a4515c951af87" },
                { "th", "7369ce389d38d3328876549f7ddff1fe7c24baad47016d48c929694315d8391e9f972be282552196c7181e13b455907a840d1d839e0ad036c5dd5e94333e1a85" },
                { "tl", "95fddf99f13364f0fc3bf7b8f769a03029d54676363104613c76a877076a16be51f75ec2e3cdb8ed9d18db0364b82dc211cf443b6dfe5aa2e49f91b3279ed082" },
                { "tr", "aca33418a363a77b35f90508bca06f734ce98ebeffd7dc683c86393cda3d4ee2ae10fc7864a91a1e0e4e611b94a4f290b27e47ec6462e6fd476a79b8e364cede" },
                { "trs", "3d1531ac39fa0ae87de1da17d07ebab0f73688edfb79595564eb011240b92a63580b8abc838214e8633e28a262179095854d887fd474b2d605ce2d0cfab2df48" },
                { "uk", "b68140685df4b38fbfc7c2912d020dc919ed41a78fed0d208da63cfd6a13183ce653f91ae3e544ce2bec960db3d311ed8562a3aa9a1129a9b87722d98df578b9" },
                { "ur", "11e82c082c5a271599ab268ed622a4721ce4df33e028c52cefc82fe3d8649cf025989c66b51b0a09549089eac01d76dc9aa3eb03c536ac4a1795d5688a4f093d" },
                { "uz", "71d86ef45bfd3b2b9a4f4739dadabeaf7196fec51773cbf2eccbb01e33c18b802ef2d1899e5e1b6acaf9ee7e9e7692ab0c9d1e3146cb28fe4fd736c4ac7688c3" },
                { "vi", "5492231fa90ca4e3b6c5c2357adece677fa0aee193184d860519a222dd2df5286273060fe548f6d4f04673314025680c0aabab79bb6808a50924c2192eabe45d" },
                { "xh", "be0ced92d3b74ab1343a71bfc9fc7c1094314a1cdd394c9d60eddc2b59951a4f403b542a14cd2590960633c33bdc9f90dcf1a434cdf6141430e167cd2f469d5a" },
                { "zh-CN", "a226e0e92a3d81e7807522538ede5508d840e6ac9cf14ac4d03b5173653394fee091fd67a12377afa2d300e7f4934307f0ba1dd759093aa7a18288c6b1d26da1" },
                { "zh-TW", "818971bce920ad1213bff460bb86ed14c300b669907b26233a702cd5561b711393836b0e4809747cfca12b96a4f2d0483f56b9144adba976e008177a48eaec88" }
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
            const string knownVersion = "118.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
