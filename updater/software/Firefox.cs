﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // https://ftp.mozilla.org/pub/firefox/releases/104.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "7c6900af63af48f223526c9fa7f6b18ab78b6b0cb43dad3f35281361f3a5423df6bde9e7d0bfe1dc87d7a4b1252e0d76c00fd9da1c925e5f9639bf5f77193d92" },
                { "af", "0de15b658536a5dce336acee4402107849d5974254aa6d131f32ce8e5e74224b1549704a154be1f575b632809629acf1c9b680bc96e97231b5166b372c4ae3fc" },
                { "an", "4b1b05001d562323c575daa521bb592b8eb575fa6bb0a830e1287ea59b38fb76db4947be7a1eec8d0262cd688c6f5bb04f89b409a49893a70c824d3924afd837" },
                { "ar", "fbf089a5b7e8c12acfd2f5e441fd17e56b1cd394ef17ec18f04d745d3758f2ddd4edbdadf1decdf9b2677127889dd8a1a0ae53edee68f6c53f405e4d8508e781" },
                { "ast", "4a59be2e20e7a85f200fba2ac85c2043c6f293e6472bf651e1ec8f8e34eeb65ed29d197e05375e3fb37c5779d4c5df370966f9ece879ac341713da21e9acdce5" },
                { "az", "98f3c006b9a6b9d5339517511abad8f7d7342e3a26ec80b5b9ceef2cc9ae1691c0643fadb44a795c3451ed67fb8814795e54853dfe981bec7c434bb68f1092bd" },
                { "be", "79fdd6e44344a484810a5053f10c52ab3e5d96e4d817c324f3c8d502318491060bdf7f2f1ebd33312d2c0a1bdbb34f0de4acf06f91806276a2d9d00d687cc3ed" },
                { "bg", "c08d039b6e7d00a95803f729da52888ac461c322deeafb6ca13e9dc4850ed813cd08d620faf5ea51c2a14b901101615a24ccdf0f07027699bf2f67510d85b68c" },
                { "bn", "0562523c4eaa75dda4fc1f306e1f5769baec6fbcfce629b25cf588a35959eab4c199ea5f3c39b45c3854fa15a545b9776f31fd83e8a6c7a1ae53cfa6c543c503" },
                { "br", "72046b1f36f77a5740aa7f74769ca011db76d7db028d4542dc47b25a8dfd9a85e6623a7df1a50ddd58ff678c557ea759f9a7eb885d41ed9f0f164de3797148c6" },
                { "bs", "dfb54e2080bb58fbcb509512450b012ff9c7c0b40ec11892236d10b4ac65af18a06e9b3934e91a4045c3a3bc267265caa7eb82f6c88adda297fc901e7fb2b36c" },
                { "ca", "75ebb1a016a317583b50975dbb7a3988fd742c496adacd373e5772f65152344ea402571e1bb1c3c8cb5ab0f1ae38124483d3ce317e07ea97bbac42bdda2714e8" },
                { "cak", "f4a0d4f91d91d1a7cedc58065d80fcd47e7e7edb5c2c21a7b6dd1643e2c0190e68c9bb110b0f225262bd3526efadeb969bdc4744dc513be91836847776f6c7cf" },
                { "cs", "cf99c4d53842557bc7da44e38313fdffaf6080a74fef3945d292357d258b88ff9abaca1a630ee4e8ba63f691c44b14e9f54df5503ce7ce7eba4a331c689a170f" },
                { "cy", "8ed82647088ef8484945a08e0788b649f01aa4f249d03833347344b7668f87309a803c04b393d4f9d2826f545308d18531f8c6bc11deca94f334584a450c6f95" },
                { "da", "a0f0ced65ded33779d10324ff98aec349cb0275490cbb23efd1dc4554725d84e0948161641d592a564f23fb0c704a46bddab8a3b5dee1a61b1f47af5fd7b2e9e" },
                { "de", "f91b68ed7224a04926625594f26b9855aa93cda6928cb5ba96a7cbd8d23f7f39e0c241e429d1e0197ae4eda57233dfddfbec2939d251eec17e4214dd06935281" },
                { "dsb", "8d6730ee5b94bb29e5ca1b82f9c4315a05a87611e08de2f390efaa54cf8fe1cf640a83efdea4ee95ea9f14e3ccda02d2fd21bd417d1267afd04f4532d55b74b4" },
                { "el", "82169d3cc9be7ab9c4aa590996f4ade6a3d0cbc2b58d05c90c980362a3a53593488fc01347d953b035fcdfa87b8d932340a27fcef6cadeaa98115315cc824ba3" },
                { "en-CA", "eb533dbc8285f20c7d89d0ed2daa429d9994214fe942747d020cdba476f714d47bf05fffa21f2f908942dd08c101024a62a8dc76a95cad04ca2ea64ad78de50b" },
                { "en-GB", "cc2a47551fea62a9e1d3fbdee8809d6bca2e6676a4312e7394b4b837327fca1e5bc55601328f6a97fae9601c7e80e7fd698df9f854810aef489458221e0b4755" },
                { "en-US", "62640e49dd109c65b0354d6f4e40183800359adaf4a885cf3b668144d30629d6f568f439493298f7d34ddf92ead355a414718bc4ec4c90032e8a310b733ed8e2" },
                { "eo", "44e301da09e86ec4ab6b55e911f6769795d751302921439c483d39cfbd747a24b0b065e949a771ed0470e778868566f4ae330a4f771d4eb945f8194044ebe7ad" },
                { "es-AR", "96f139a5fe7ffacd5f0bfc2caf00c054f566bb4694e34a9614ba68581b696f0a5cf79c4ae7a9f4cd9ccb694b9f285ed548e0b31480956308609d3ee98a92f21f" },
                { "es-CL", "de75d41c6f4a992186a7a11d38a7d2188540a78b9c5ea5e49b81902306bf7ba39589e4cd48f992bfbd9e72defd6bae0591806ab9c5a54b0cb23bd99c55e5e913" },
                { "es-ES", "531d9912655794f27eb2269ba3bb651f5459f3bb942502feaf2cb80267e91a6f1eaff8279d8a493e1cc534f41d6056318dbcdca9f2e56320c340e3509f5dc4e5" },
                { "es-MX", "4221f639a3142a25c4b0b602556a16562f31d6d2942400c12235108b2c79e2661a98dce98032d6af3ec7f271b50bc37409aa538402735a90486ca96c1b72a646" },
                { "et", "253cd95b602cbcfdd224751ef09192433576841d095cabbddd4df732562e4d27fbd5597bb67092c267da79eee9fe67f82f51ca8d08823f9052a92ab4df63dcc1" },
                { "eu", "401f3811a7712c557242b470850bf170ce29cf12518eeec5ade4adabe24341f06886b92728217be13886706640e65e78b59bba17d55e81e091e651687a88dbed" },
                { "fa", "3899b391231339479e86aa493949e8118599ac2dc59b22aa34bc6f7a0993028b24eca159c09446863f72259fc56027bd6b18c55c0b5e7198cb298549e0ae2da8" },
                { "ff", "308283383595de274f05eb5c4aeaba1c665fecdfbc960fcf354324fd79236801a350cabc3a604b45b9fc245666e9784c87223a9825936d778f7f4fd4f5fa12c7" },
                { "fi", "5acbc6cc1f86e0e362b30740f2fe5d9a7ff5636adba56662280792b1d303166dab3b275f57091d7751d7aaf4fb8cffe817a153e672bd12963c03a5571af2508a" },
                { "fr", "d2e1ef9e724aeee17254dc745b74c2fb159bbe59438769c12a7dc51f4eb310f19c4b18fb622faeea1accc05a950d5771a90b945dfc5751306743f28d2d7c5a0f" },
                { "fy-NL", "0146ad30bf206052419515af887d1bbf2c134416ce49a7aecb97ea678357ff7523b9758f4ec6c0e9bf1faa34ad0b8407d5e0989ccf5c8924a4357a6d6763994f" },
                { "ga-IE", "a8fb922612125cb05dc72783fad206332d000094d8de6e9f1f9dacfe1084a9c02a846eb15e606dcda56f7310a074f47c7376982e65ef92a5b859dd72c28f62c1" },
                { "gd", "2173bf68fb8e2e5e372823fdeccfc378d4f25cb7e976e8b1656a0a69af0ff53b1cfbf289ab38721f18b3b5eff144e196ed43ab279494a07f916b3bd6a66e45e9" },
                { "gl", "29424f6d2d6756250a47d80f106c77932a16f69ff8a191fe1c420ef32a7e92a8aba84bbbe44c7085c7d7ce741465c5f22817a241c98b0a293ede93c71a6ebd00" },
                { "gn", "7b6be09611ead77987b225ed1f7b668dd9d880ca23e4fbac477cf50ca17867ffdf46cbbb0c5b2b571cd8a94372fbb0b033578b4d5d8a14725fe4010c43d93b25" },
                { "gu-IN", "15876455dc6d56ad29d9fd2937838e4835f7b1b56574f44cf82e8737289150f7ebaca8da0d2f334e0b9fbb04573529b9e341b33be8c3f6b8792e28426fe019c7" },
                { "he", "934be2424b7f1e37fccc07b84bb0188b83736fee7a7534300b0f95ebc9c4cc33a13ad798c397a1619d962a437a2b2d4cb08f75f7092c0ad05dfeacd862e0c8be" },
                { "hi-IN", "65113d3e43cde48a7d8b1e825ab0821c29a0f86230533f014cbcbf182dec46b22567c3ea1193da43ff391591b6ef9fab84b87317384f33cec38186027f2734fc" },
                { "hr", "0084fcb39292ec6cee05812c080de52c8a5e1847e6bef4d7842e1a2f7c8c326410bd5672cf13659c892390b4d0549f8b30c6d4266f71f13030cc01802eafbe4e" },
                { "hsb", "9c979fc07708325d82d66e861c30b2aedb924ff5a9c7635817baf1f857b6d7bfe77158dfb22e7d5d436d3de6b1ceac34d4307936c0a31bd56f4008d503af45b9" },
                { "hu", "39a2ca59b505470feaea4ad470710ff0c6becee4ac168fd0d698cb1e8edbf59e685454f555fb62f5df98223b928fe1f0b4f8978addbd9b80f2d1af82ff2aaa77" },
                { "hy-AM", "499f39f38e59d7df7fb25a631e432d588ea10bf7d60f945e7bc5fcaf209e3188307e20ea27870977bcd5e1c60037a4f755e715c30606034cbce875fc82776acf" },
                { "ia", "6c2b4874ccd7aeda904afd9b797a1cebbdd99627aa1b740ee02d0f112eded954ebd6ea7bbd53590423569a3e52efb8b1bfea532ee906a76cae1241cf8bc32648" },
                { "id", "9d113ca35cb276199947071be79ebbce58b901437a96a091c76fb97507bfe2e5de446891985bb19978c03ce8579a913762e409e7b38d5aa7ed065b91c7bf8f65" },
                { "is", "dbbeb60d6b437f185d8cb1fd353de0c953a88d7e85a32dc2ac6769d2f5802b650ff287d36155a95be665d69ed3b834cd11aa3e40aba9f27eaacb158bef6d88fd" },
                { "it", "92607c1cc7beea6a0c41f13d91738619120c066cc3281b6a140349a8d56e261aaa52b3614e4fce0bdea993b38b12d3e8c06c1de5121d5aa852c37487d3f665c7" },
                { "ja", "c20ee3e8b19f3ca8869d7b83c45b82aca48b1b2b46bae565042be9f0db8f56d7ff29f1a71216f00406ceadacca2180420a8464250366d85be6ef6dc226cfb5f9" },
                { "ka", "68abeacb7c84af8ec487e87af04d6bf65131c58ae457af2e298a72e2e3a361a38ef82fdec7d43e1e573beeb5593d749c9acb33e388728ae653c2f52235d132c7" },
                { "kab", "30b263d45375ebf47bc92b270f02704d9113ff78d760baf9fad331512712331f45ccce9559336b894822bf0439b4fb3ea52b7128f7f8a2a1bd97a0945c5f4737" },
                { "kk", "2d27a022ba9bc67c8502be98cf43611f8c4150360432643517039ed0158f0bdff940171659b03d69a9ae807186652191f4ceb37209479947761334e2243f3261" },
                { "km", "6339eee56c377c8f94bab360b01b816362cd15bfc2c4c044a26a1b02be0ec836b72b463b2ea73fdbe7911fc0fbb2c5d9f5680a550d05edbe497665284555ec71" },
                { "kn", "8f4efe0098c239001544954d770e7b118db8b03ae6793925adb9cdc70ed231127f2d6dfcfbfddfe42a8f737bda58158978fbe2fbd3945688b47a9d54c88840d2" },
                { "ko", "5325840beb1a3375f077d752f0432cdad2efe912e12e8dbf7a70100559419217a0457f624d17af83eb1ea76becc36ef2f8031d421ca301814716dec4ff00c49b" },
                { "lij", "389ffb12641099a1b86f729be9957d0365594ad62aa47bb8f5124992cab2de03c58a024db60543de7531e47e5f510ccb5ca0fda6255b6c2f7f6e519628f5c9dc" },
                { "lt", "c8b53f59b6b0b12fe8afa26a420ef57887bcbe588c068c4ad01d6121a0fed93201e874d2cffadc6293e604272cd5f7e2d4c8f2752874073e35f3091d83e5c869" },
                { "lv", "837b3f02db82a7a389862c49785ffbb43096a543dd053dbef28b2697aebe988162222b01cd557afed2858b39c1582909a65fd9d628bfbd0ae6123d86c0f774ba" },
                { "mk", "732f15066a5469f548255887e3b6687ab8e8a974c3c5394a3c4dd1cca8f2f6a41a0ffea9107620fa125c9308463c165f0140682fafed54e7a89e58f247add667" },
                { "mr", "db80053b14d8f7987e63ca21e64e83892781a2191cbb390419454bb5f3a055126d1afb6c16e10768bfb111bde50b1861e4c8014d6b9f618aa135aa1b2e602342" },
                { "ms", "7c4c7dc1e3b6f02416b893e3d886e6e36c1c1472bf8dd4a94ba5346306872ff0b9edeb91e36ab7b2e607577eb8c5476e3769d34a85400e3e5e485214e1525c2a" },
                { "my", "ee93b6abd5bf57203a6c2db26bc0bf81cc52a4c7da89c6206c7ccef3186987120f89eade0bc89c63e0609eccfbb417a3794dd485f08489f516ad9012e1300c16" },
                { "nb-NO", "5c017e54dc358fe83547e160bdd3a2b663d3408d3e6f58cf749b96aa559c70f03ef89733ae3f1a1a10ad77851c99016660db39f377ee1482f9f0725cbae97e68" },
                { "ne-NP", "62b1ac44944f33eeb5c19257ef882ddc9d8357eac942c96e30ca42cf74fb0990909502c51b7e65983775578d2852a4a0b2a397e43078c9df09d09a5a312e34f8" },
                { "nl", "3aaf19e5008b149caeadd62f7906797955af2795d271929add691f554654a7da7d4cf549fdebe8c1734e67a46e039f279c20fccce2129e4bdeb7d8aaf1433268" },
                { "nn-NO", "1956976d88aee4595aa9339cba9c37ecc2d0987695d05939f72c8544b928c3e2d71ac7b98e1b213a8b2f35b6ab4dac5a2ca08fde040e0320616731439b864835" },
                { "oc", "461116b6ece40fd3a6cfb9476d160e0f1dc832e51dade02cd743b3e0828653bde700a44ce9ed9e5885d960a7ff98ec61eb3b9d6e2e203e2f3e043bbff6e784ba" },
                { "pa-IN", "ff9e627b78ef021110e2f6b6a7d31835baa4928fbf8f0a2281ecf31025557f7270f8fb5d1c4275ebef58a1505ee52e21bee731f518559522eb1d18a33474c398" },
                { "pl", "fc920f8ec97ea0f8d5aa1f78cd68380dd55e707c7b0175aef53776b6337c86e359e3ede1e3805db766ec7a6040d866836a6afd8451507c149ff0be868bcedf24" },
                { "pt-BR", "3b3ddee0bb5320bda0c35d21f80d84c5dee81d1eac8583c1222fa833de236c367752df687a13ad74b1e6fc8d1a674316c78a0a8f6046f23c1adb0875f882fbaf" },
                { "pt-PT", "d179824efe8c1aec44274040db818fb2e31a7c96f8fc27dfc6287d1b0a1b69a1c2bcdaa9795b6ae1d7469f2315b1c1aa31e203ab5bbdf83541c84051ab5829bb" },
                { "rm", "0b6ab3e2891e0784fdb7ff5d27be433592496a8cfb4f3980d5bd6da2bd9b2c1cd900678e7906c53e6f2081429c2fcab06374cae51f3c57e8574a1a4605133ac5" },
                { "ro", "61197d46be9223f46b8bb51665daefe0435be20bcd78d76b528425e2954901770d47d88d7f4bbe9dd89ecc1180212b8314fe5d5ed4abdd69ae7af416af84f22e" },
                { "ru", "cf1471091d6c3fbc42b9f843e1d6b509200029c2d037828b3332d12ec0b1cbc4462d5456d1fc0d7fee26d168dfbab963db22735bf828a043db723084daabc88d" },
                { "sco", "e23b3d9862ccf8f71d1194dd54ac3606d433be04a1c04457aa398683bda2e40c16683b026fa1013cd1332a8a488021c0d05f3f9d4eac8fa08c6a94b63f1606ee" },
                { "si", "8189ddb27af50f4f60079b0c7fccd8db726bf9f67642d8bf0f96975a0e54bd7703888fd8c691a318e66229e7b2c42bc6ef9e12e1a9a15e84b6cf34b805559524" },
                { "sk", "8bc58440cb81e612f6eab6264bc1939f94168a4b12e64efc1c46fa0b98bc2c0e87ffb68b54cbbc8f0398a44f4855e0abf857f7920bb4c185cd4f5c15bdfd9783" },
                { "sl", "de386e987042bdecc948075881e9aeca0dbd87d14eeeee4788292905487ec80d13862154d34ca6246881c4d672c73dab3c147d20590b2e1ee708074f8b30dcbd" },
                { "son", "b8826fad38dfba37812d2338e478f04a9369f1664f8840ca22a602d888f25d8eddb2848b7264fb040f80265776c06c7f832a18b78d8399c97534eb7046e3cbd1" },
                { "sq", "ae324532218965f815b4695f1844d0ba7192cc396dae3e9d152ef97da1efe3f11766c59ec9b0df671c43433fcd51b9322e4763be5f369770f8f35faa711456d5" },
                { "sr", "8ed8b6e873ad9a2f896f4d867cb25cff81d08b6ec169c861a97f22e2acb8ab0d251f2a39a5aa8c21bb6d71c9f975a2866930d5a1a6c9314051b64653e4a116d7" },
                { "sv-SE", "3efa56c7b134fe6a4ee0e7d03c4e389a3a69c386e19367a1b121faf604815a1899bf4386aa312692871fe85d766fb475581e0120bd187987daa390795348b7d8" },
                { "szl", "20b3358c7ded5bc72668fba6b0affde405195ad6a18f6c00154c60e8cc983f2acb14e93e84b15d8287e81405d0430fedf188a486aceb99b4c4a02a7e03a874d6" },
                { "ta", "77f20c015c203412b80b818c8dd3b4afdf6486874768520566e7aad71d31a05e31daafc50103ccc1c25fc1665ca6e9d134b40e3b750c95c4963458f4539a8c18" },
                { "te", "a3caebc38023061d3f53712fd4ccaa6441fdbadd9e74f93a4df984ed3ffba0f01be02781636a0ec4deaf47c39acb5252bef50b466e2b1317a0af4aac92a38b22" },
                { "th", "46d1daaf2a95a75edfd8ea1995ffab07e88a8ae5ded2c99aa9dd0d2825d9d2f0768b35cc058e5bc67b06b86fb2805e536144c625a156879db540ea996abccb69" },
                { "tl", "34c86d4e62feb1c15d01bef06eaedea198b456d94e6d343004e5121adaf114191aa4b96c91e0471883b15e8b68d07e1bc0896d82d9a63417c4952dcb93b16c47" },
                { "tr", "54cf5af00184cb70d10322a11bbee22d1c008a80f13f5e365a760397c04bb7837754a0500c4307d1db26df0686866854066a5c543f38d84d4b5dc0d50d862c60" },
                { "trs", "ead716ead59991a6cfeeab1b86a50ce9d1e8e60a6ae7cb815ba68193fd30f47b5cfce4840fc0d40727121101ecee7637ba8b91cbfd470c283aa34c39e90dabc9" },
                { "uk", "d2a806aa87b938c936a05fa1bf4ced27eadc1d79e6a7c092e8f4487e55a6902414b9b6a73c6857d0ce056400bd22130c09d8dcbcb7e1708959cde38cbf903d04" },
                { "ur", "dfbcb6597454f32c95b391665f4a2ba64435664a5768856e509ab3ccb3c80c316d905ac078172e9cdc438976dce47c7f68a0d1f88e4f310676816a748f5397aa" },
                { "uz", "ede5aacff2f7dbce8802d65d45a0a9494c82c19252184ed348ba6406ceea85c7170e33811b04df4d5479f0cf60a8d18d8e4c13005cf365beaebd54e3560f8e42" },
                { "vi", "9960c93206c919e5dc1e8609894e698e3a1a180e0b2e63b0ea8b92c30ab128c9bc2853642bf5638b295b44d83c1e8b535c0b04eb544e38c4e1b2e21a2eaeb0e6" },
                { "xh", "da6f50cf142ced7c65d33526b5d8fc694e499b954260b3ed6131bfe2f9e2920390df1f50c0b076e1747cacbf7db803bb5d4b0bede32e1a93bc0048fef18e2c56" },
                { "zh-CN", "7067990b8f154ad57c158907e2c8439bbe8929664162b4a68462b148849389a8404742165d728eb3fe8d6e56307186a9fd6e51d534c2b1250be6dca1ffc0c765" },
                { "zh-TW", "aaeb0ad9d215ba8a5023bfc38726a957894afe1f06caa123a0b75a95b157bd627e74b6a5ee1c8fd79d1ad03e84b08c283ef82cac430a0c6267c61a37133f50d1" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/104.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "675ab1688cfecb5d4ef0f53332a94c050f3fffc43b25ca981a0eb66189df3e31a3177521615c3cee0157cd817421bd9efbaf1b8a618f483a786f83ff32b61b89" },
                { "af", "33b415487d58129e984eb803ab02859cf418f4f090bbec0629fe6322cdfc150f209148da73ece8fb4bf36938b6f77afc6173110b32e4e8ed48a1b3ee289c52b7" },
                { "an", "828f640c045a7f6f1dfb10b2d7c690bab7af47bbaa7f82cfe76642cd1169e307f7bfedb5f2082a03a365daa0ac1c47035ef4394036648226a2bd759362384b84" },
                { "ar", "cd950cd3c90883d558689026ca3e82eb3593e2d581e27a06167853cb8f1059c2e5a68457ee46077ddedc804065572b12ce9096bce1279a0471027ea67c803ea1" },
                { "ast", "714dcdb5e3d393ea07a353c7a2a13f7a895bbbb0bb4fc4029698846b26c69e71c3033f93eb8958b389d5845dd7a48bd831453ab7bdd7b85c4f02fdc0f07b6a86" },
                { "az", "576eda834627f3e5dfed760f9ffd03208b9e3642de1754088d18268b4d16c8d5061daae4fa95eb2d6f140b5fb7304e4181850a0d7dccbef79b940bf339995ad7" },
                { "be", "021078ff1c8ca2351b0440c67f9cb090111d3534d8da9202427d8e0f932ed4e3b53c12c344ebdc73c2d3ec9322dc4b1f96b309b44f0e56f106782ad05a324820" },
                { "bg", "1d0828b539eb38f331556a21244d304c3555cb6fa4ce0c976afd1042256375d528b1a0e46d80552fe950c72b015ff4f47005cf7f53db2199b0ab8ff952239f90" },
                { "bn", "fff399d63a166619187a8ee5ede2679456643ebfd694362eccc74ce36517fa4ccc9632863694a47582536c0011625c04af0bffb159f4c0bee8f110c2e0480cf7" },
                { "br", "b1757e2ca80999345f64fd202b5a26e3a07029e76a6f1803edeaf1f6f6bd4e61fef76f04968347a28e18458075c7a244cb53544919501976267c59a82083b1b9" },
                { "bs", "43f8ba5d09596993c717b3eca3e132ddb6754f99a2b05dc7e0b47ec155fa9f5fccd258efe587773ee7a3e13e53802f4e61a4fc9d7c1de414b9ef12548cb794ca" },
                { "ca", "04312add3daa3295876d06202ace046210224eb8729b7349516bfb566175937a2c89b2f5fc4bb1e94e6cfa915353c83caa3ee66af66b28adcbaa971afa5d74d0" },
                { "cak", "8c9cfab6d3f722c6179bdf522b7c6ff663c5c440edae0a7ebb66e686f07d9e6405afcbe361cacb1074e8e4544cb85f41eeed66a6d8268eefe1ab88d0d5f20438" },
                { "cs", "209149c3a59055fcd9c9e6688d98db98c27c7df92ccf1239cea1c6f618188523c6e5f5f43d6b2dcb97c968e0b362f680c68958fc3ebbe5c14a5f09f023ec0fde" },
                { "cy", "5cf6581e5c559d94c2dbbc45003d46a17a65c93e3f157162cd2836fff7fe68f0f925ba1716e7253f9fbbed3d584c0a05667d13e2e5955b56fe71e11eb6076adf" },
                { "da", "15790abdc1d3f88c83fec84af80f3c12234089196ba84cc033dccb97b5110cb561051cf0c5961be09c924ede8091ed48cdeb8b31a3a09e3ae8d991ad4098037e" },
                { "de", "1f640e36f38228f3d9fcc68528f5bcbd86047719821b9007bee03f774351aa3ff9347573014dfcbb3bee5bdb082a74a30233fb6977b40ac6b0809d67a19ef4df" },
                { "dsb", "f7b7c6751c77534dcf5e8ec9b7d3f38656df8ee9910d4dc101e32a8851cc993bf7f8914b4028050bd2ab9ce93ae9e0ffa0aab2c3de627e7bfe678b5357736de6" },
                { "el", "380bf81b51766ebe27ea228914fd05ac2931711ce7fed8538816414f6bd1236d0a4bde85d8e41a210b3a83a8926340e5dd0d71947d8f46c9987919c34863c3f1" },
                { "en-CA", "b55b8afcdf43c5a7372ea1267da13dc1f012e1da2f635f7150f357415ebe0620b7c01706658f04dd2be21ee1ecb1b4af0d864496d96ae8b4ec1c7eec00d6cf01" },
                { "en-GB", "0d69e710618ed39be3d5586487ca0fb424cc6258887051d801175cb313a3d67761463c289763422c07d66a121389dddb54e38b60cd79988c7c5e5c0730593c3b" },
                { "en-US", "1b2945599eb889d793ffd2fb4bcf59ff75eebc41821c05d8ae6a135d11734aac768e457faede701288fef9bc686b416ee7780315439bf96953bad89f99e20bac" },
                { "eo", "d5b42237380432e11330b8bc7ce5f648f3603d156938083a05eda81f93f9c95d706c33216a59606ac973177c892a7574161a6d92e9da026b91a755acfbb3ddbd" },
                { "es-AR", "d2b6e7f1d73ddb7201a1c0c20ab1bd1c16369e4b6f73b3289648056b791dd962b9ead05b8b424c71c05d840a72748a7c7d7399f976d3b27bc268fda3ec2d3f9a" },
                { "es-CL", "4241678f688908ef996ee249223f3310af50eb068e830a85f6367b8018f80a46f1ed0c3491ce5f2713e378ab097953834613acd5dde8eec09f3e6e9564189385" },
                { "es-ES", "e2ad61f719402a6518ce6f99db5b7f626c3002d640c1826416308d61f4949ae2f388488cd4934cc9b3c75d850be74d41c3e115fec38844066d3743e22b70a751" },
                { "es-MX", "4730701dfc943dc622afc754fae644e7395ca544f22943d3adf0333701671360b84afff48388724e208b601b0832da51ad8e4ad6986f82eaa1056629a1e4305a" },
                { "et", "bb6ea63b60369860b05deef65e98e5e579d16972ca13f3fbffa8dcad382c799052c4db9d4b861f36807001d4274a328569d32fc858604a196dba33b18f64bb42" },
                { "eu", "8ec04b78cf894586a74cabaa400175332c79fdebac82d2497866aef17f5e0a5f9f2c9351e0ee7c6331f7d25bc7ad77d07d931f42448580f583fa0767b89e5568" },
                { "fa", "a9aae547ce92fca4273f9e4e72ede7c9a95423276c414b0cb66e671ed80bd60b60746f2ef630b1a3e98e7e1bf14dbbfbb0529aa60aa9e492133df07c86dcac31" },
                { "ff", "f9d0da528b2d1e0c5fd966e0d5757465b0f7599aa36bcae1f860a7c5578b9ffb0177d3dd5e5ba2634878d16baf43f629297f4b3893e4ec0db9b368d52f4dddaf" },
                { "fi", "ac40c4e9f000322f587925bb62c8cef0688840534cf26a25075d66a2ba214175e2043e59d2d855753861edc52f064ccd986958c028b6be4bf655b35519827828" },
                { "fr", "845fc0454c7b370e4d480d1c040629128fb820b752d2d3d6efed98b69f00aa81ddeede990a57afa441b82b4f09ffa0fe85eac82880d09bbe178728f1f23e65eb" },
                { "fy-NL", "57a8336ff5d7c172df82834854ee6137d623269e1a228cc99ff6e4aeaece84234ddb262ce2f86727f9e8ce6dd565a3fa4b8edea1c43395a78f6b245dbf3b684f" },
                { "ga-IE", "4671f1cfab00c6703980ff8f8a22588b211bb33b3e17063c078c3f16ff72e854a2a71624546855069056da9bffaa1beb4847bf56049fd72b1b0a17fa4e7591f5" },
                { "gd", "62e57c813db6058f67e3b36f82ef39ec078007ce4eb2a14fa8c3eb9bf5ecb24a8b3edd035d5e7496c5e62efafc69c17f666279253e4d004d52e576e7ce7f6d16" },
                { "gl", "0f08322c31bfd8d26979bfc71ef2a697a5092a5000f9b3607ca11e61d112251e84ed6221e0827e79b5da46fd4b99e4af1bbc0527afe1bff308f9a2111612321f" },
                { "gn", "4eeb47c9ca55046f78fba5ed9dddb98b9de77d5752918a79f0dbad20f460d019972af132fe7710994bfdd891bb29342560ae4b8b4cc0ad373ba35b5d345d543e" },
                { "gu-IN", "91d1d52aaece1a31a83f9952731afd0f44019cd4b7cf9de72d2e32e7ed3caf38dff02b1ea8d44e07656b33d1d10865cd6cd1e39209afa040e429798a4cb35f3c" },
                { "he", "570e686d1efa1b27d14baeeb0cd6a12d1342f9dd27a33bb82795610b04f8f2082fec7c7201a63523043984c0cc27a0b25d298c262ba12f61680b65d8f0c5722e" },
                { "hi-IN", "0f14393ac62ccfe7ee4a4e4a1e2a958075fe10177d94a8bfe77e69ed2a3463f331af8d411faa0f04c39570f103800a0aa7c702359acc86d205e9c5a2c0ed4d14" },
                { "hr", "65d37cf04934fef33fce66810ab5dd636f4f43ebce68ae7170373c60d6e82eba8f21023680da0a7cffef42a5f3b5f1193a300070e7582fe6bcf4a663264f303f" },
                { "hsb", "3341422e754e439e4ced0aa797538eae673759f3fadfd8e78a77f2324eef6a1b8ff27676b424f6471e30425bbadf1138172ab18fd807581aa414440de57bf061" },
                { "hu", "6f29c4eee5f4e0c5d608941559e2f57c0618eea839a0aaea2491ad211a58f9135d0feef10d6c54930ae725e782afc76aa578bacf73552e62a9aea45836cbf949" },
                { "hy-AM", "c340eae80bb3e01ec6ffa6c4d79072ba2908f57465f40f3fc91964c3f3494d0ed0bcf1835624473b4bef04647c2c8ad1ecd766c9fe977efbdf8358d31d226dbd" },
                { "ia", "6e72cda8668bbeac332c5b2a63403e43d69e3b3aa38878accc2da021ca576a1ddd08b344d5ab6b976fcf92239a248bbdea61fd977fd93af94b9549f093c269ae" },
                { "id", "982b70e5cbf289958bf94fb88a1d4eee2b9c1b0b12763f1078244167ff2ab1e92e6c7bccdc48de422ffd471a4ffa41745d1e9548fa23e38d9fd7b4d046f88ad0" },
                { "is", "c97644319a6b90e99d9bb59bb0307bd81b3c04912cf745c939ba15445ce4184d930cd2b97c719a6d596c16893f06e8f9855a399d84f997bf08727193ffec7e18" },
                { "it", "32c09aca1743b7066a1a3253f9e3e4b25ed3ea17839ca41f8d73d7671a6669f9856d61c43c117ee2c573af10877977ca8a89dd23fc5b3ebb4c52caa6a457adb0" },
                { "ja", "c73060a13c54436d4d13c14dc770bee85b8ee6a3c76bb75e8f47a892730dc93a24b3eef8ef0dd624c7236d1674bb88362dd970dc54fa62d489a14bcdb96d118b" },
                { "ka", "6fa4faa6214883642a90dd6d133b1cc16a105005046fdd189eac96010bf70e0eaebafb3b8da61a967dc8f0a6399d7b7f15f660cfe5f0fb2145fbb04e3f653933" },
                { "kab", "cfee7af2160b995296873976f67c07ac4841bcc8990a381cfc4b72896c1feb1d7949c53735ed32ab4fb717f401ef90b33ebf21063f130141b01d7046fb41bf85" },
                { "kk", "40c6e66ff7289c9b2af54e546b6692642489217d8fe61bce7783c2f7beaf38608dc03da039cb92e15f67ccdd68e108a27054c4d5cf64025d9384dad3cb95e4c8" },
                { "km", "d3bae67abce06ecc81bc23de7b2ba775ed3e8d1afb51ea059fca6442b8e30b6e5c265e9881ec395495c23fe54d5bb9af4bbd0045ec097b6636872bd3b5373bc1" },
                { "kn", "384dd6ae9a7a7a8e41c16c0b2808363b137bee6607bd6afa88c6efb74a211c281d1db789369e6f1ef52506d2028923feef8008c3fe67da7c9bd73325b8142970" },
                { "ko", "40e3f027129cff6a792e06cda6326ff74091fbe41e27ad63495113f135bf4105dbefa2cf840cf980d975c5011256db2e23799f8d3bf1f285947cf60d1b057b23" },
                { "lij", "bb030ce1a305f4a6364665f9aec3553922bd6891dabd759242b3744349c61546b95071170c521a8426d01b26ecc7648b3a4ca310e99cf41b04d887825e9e0334" },
                { "lt", "4031e994c7144c4133695ca2f7017ab4a8d2e880b3443b19b425e3028c11a6d00e495c9843d49485a1a048c7d75780864a85b58102930021ca048e3da89ac61e" },
                { "lv", "8932ed958f80798b77ea6124053bd806d0436064e7b0757bd4e7c13a94f02469b76e7aa6cca9cf4604f31dc8af070df9d8dbf04fd8d6a6cfcc9ecbcd8b9bc423" },
                { "mk", "cd54af9346ee054af8851f23502e2331d38ca9f4c9d84ffa0132a44cac3454ea0720874b86ae4fe5bf22d3b5529ba57cca1e2635164ac499837f3e3f18a6973b" },
                { "mr", "b438463449a5f0310df594d58ce89d89d912ce6248bd2bbb460f1bf13e9537bf296b5cfe4969d0fc12e44c174530c72548ee124f33c6382516f33917224f6b2c" },
                { "ms", "641c88bc7ccef6ebdf46f1071370df4363bf0e279e43073e741208596ab2fa02f401a22dca49622c28c256f44de22f8bbdc10d0efa21092567d3e0bfc3258b35" },
                { "my", "d809738990d50f28844ff5b5a3704b785da64f4c33310e34839dd9d5dc7fe8b3cb771f717658857e45d416e974820dcb5e07f8e36b4213cd728b0fd80df2bcb4" },
                { "nb-NO", "bcbecae5eb2d24121e1c369e42768c98bda7b744d08bbe8dfaf246feb388b1313311a248f2ac5ff9e614e0a72ec8c2f3b62b8d9032ede099b80e38ee221b6b15" },
                { "ne-NP", "c416407b9ab8922e5ec90c27e6e554a4c48362304911ab01a74f66dfe12bc46ed6c8a11646156f30f2b5de1fb561857c036f07f2ee90daca2f7434bfa0ffca6e" },
                { "nl", "855c476cf447464e8f42e7eb950c6dff36b1600334a4752ac7c1f8834c899a1c9be34860e74906d53946eca481b0ebbae1c373ea9f9903166fec152073f62390" },
                { "nn-NO", "7bf592bca91e743b950824fcd7ff67158410ada71420f03e111f1ef7b7e0c5cf98f1dc2060691469ff242fdccd06c604a5eec1440d15a74ca17e451e8fbcb058" },
                { "oc", "6c330127a583b37ae5fe31e4e470f73c2ed9e532735eb0681fdd6c5c7f08342cc7dea6fa7ba4f46a1bb87a59cab52c1da15378135e4101b774e5bc676fb8367f" },
                { "pa-IN", "1982e4d0240bfa711407a583cf22e98610d42bfc9836c03fe37ef85df05993a1902b57bc12634c64221790730aa969dc831ef65d9ea25b665057fdb3842bf92f" },
                { "pl", "ce79dfc367b9cd83b42e5f8288324445fd38fa8bb932b7fe9aebcab937e0139b58c2c3c732486cc83fe9785c73159ae165b85aacfb23609949a4375e352a308b" },
                { "pt-BR", "6ad178e8594097faf487a4d473d90e3a250a4d0220aad1616f92625e0d407c4d4c0a763d0fcd7d5a77cdcb64bcf209c75d06453f4e8f156a4808836b2d2e1167" },
                { "pt-PT", "95e8feedc27de20ef09b391c710044a5849d22d1e01598a0dd27d0db79427a0a5aff89b861aedae3494ec6f1dec016aac700430f8382e5b3bd4dc80c15a658b9" },
                { "rm", "90946043f3ebe8d74e4740f204660aa2d37d64ae2eda01b8303ec0eaa359a9f46e96f5c32d597cec69a9086c6b8e86bcef6fb44e5474397028cafd88b704ae12" },
                { "ro", "2cfde808abd40ea38dea61bb01b59aa919a60a0a605d6423a526d956287bd4cca3c00cea56f669adece4cb5f5e31b5b21d2993443327063a7a766028d4357c14" },
                { "ru", "aff9c999c5130ce0bc807768a471d46227341eb602a26c2da37241a6bc5539d6d66784677765946c2c377fa498f5534ec5e7bac0bf9f22368f1cfa1ec1a4bf56" },
                { "sco", "f2131b296a295fd3498bf04f009ec531a83b7568911a12ff1f1d45a2fd144cd45f34dac7c610d01170cfd83dac8efbf2a6a4dfef18824141fe94aaefab421a11" },
                { "si", "f351e90931ad5619d325404d8f6c0d18882ccfe5a6df675e4146888bf0bc398662d3f93f61cf991302c28706b65533ae0a45abb9c163081586b045a3a9ab769a" },
                { "sk", "940461ac9ff225abbc7f9c5c3ffe899422d3d6253759fa39bcfe4a37edac2b7f1427d3f06b3577a54d8086c00a7b5f8d8dd26e75dca91f69b37d20cf41dc8ed9" },
                { "sl", "68f398d07076047c7827dca56e45eb90aa2727f9887e440ee727ca38ea750ce290fa26bd252b20b44741a92576833d7356b5849f5d8c16dd904bfd3f05d80d24" },
                { "son", "ffdc0e416dc4c239040c7cf8c452e33ed19d5fd82a59f3baf521906e493705af7363e80be28aded0aaceb34b084bbf3db15f0e3fdd6047cfc598a51a3768b2fc" },
                { "sq", "2fe58b1cb07313d5bed80447bce64f06c75489e258905387c9706e95ea526b74ef175df9b1b0243733996d5b702893e5968c3a8ee5fa867174f50d8d12a96c0f" },
                { "sr", "1ebec65abbd6f64120ce73ae8f14c422384905c61696c0cbefcca0ef350be057583aaf3d9e02802cd0d9213d434cfcb73f144cf242ec17c3952169e773ddaf3d" },
                { "sv-SE", "a75ea7a4b527fac82c853a9690d4460131ccf75e97fbf7dd495cc22d2bb4fe2282cfbd66efdbf582cd66d4bca0e5073bc4d9f207e46d58500fd6513da9d43157" },
                { "szl", "8229accf2e7084e614981dd1ab79803df669f8e6914795ee8ec9e8cbf7f298d1890d320549d514bbd1493be9e7116e265eace2829e3218680fae86254bdc883b" },
                { "ta", "cdf5f36a07e3b558ab05ddcfac20a9591c839a3ba33009b49b3567a8c047f25b75c8cb5f332aadfa6293a4f8937db5a0b66f22883fbaa339aed29c5cdc729442" },
                { "te", "eccb1234fd040b76ffef2ede3249a6110f8ab8bd09d6d274258d7f44bfd5e65b75fb4c4bd298ed2689946972540a5cb8bf952d028b26696bcaae43971255bf78" },
                { "th", "4d180cadf1c7c248245773c018fa58e40b6768709573b63f6ba011118e91910a9f71ac09aa1b7b7343ea56dd1c437f0997502eb0f28223b2d431b33b517c7504" },
                { "tl", "5336fb5b5fdbdbd544e389dcda9577bfc6b5014c8bb1ee81013166dfb72ae5a86562eb46001c9b4f5562607b98078071425dd69b9474d74efb8ceb91355c5950" },
                { "tr", "14510202eae24fb65ecaff84f7b23ac1ab8a43ad47671c06b81148608078f42b69b849327e3a28749f8aa781d5ad3ae538fc174e725fcf365fc87e16dcb5ad07" },
                { "trs", "e21317b3ac19b225ec6f6918823d707b088cc4d898cf53fe49e7b6f28eb8cd26f443a77985ce8fa0591d1573b4ae8f7a5c21a3c32d1d727d0b3afd0211128550" },
                { "uk", "e89363505e9ffc9ad84a4ab40ca70323527357104969c80a6a76cf912ad45a7805135c24f5f4a4c1ae7617136d5899d5df0e336e9a638741affea48e31c62d05" },
                { "ur", "538dc4ad4029926570c72af86385832c2eb72e91336c40f979efb67795af6ee7f6b80210e0dab909f4cdf3c322226abf3d231d62bfddfe9447bab0b1a61060a3" },
                { "uz", "4207aa5738589d523e75a3b8dc95ac9060e16a993d890ec917f4edccb6c5ee24e4186d651ed6ed1a5c2962dfb0c1df3fbbe612c53a9a0d783eec2cff0912a31e" },
                { "vi", "94e6629cdc1b433164090f2fb122681f4eb5af4dd531f389133603452ee5fbf7d7adb3f1f89c34e9f5dc63858a69cae8284eede5331ece51e037d28321b5784d" },
                { "xh", "f42f983f833afe6a0c16237d4d881897d155a3e903a6a048914703ba5eb47e3e5d76ddfbdee6fa8a393453aae748ffb85b598917bb0403bf3f32da7bfd9464fa" },
                { "zh-CN", "340de609eb10e5c5be7abcd2ae23ec443e2c0f3de4e9d980158af59eebf7c058af1cc1517e08767fe2c8d8b52a3fa7a3031cd1cb6c130824948ae85d3692fd1a" },
                { "zh-TW", "47ea89c36f19325a493f6a4601078276e402eda7ae36cb5d149276da016c984f29bec9396f7c32d71094b50e362b209c98007883d58e80b85896221928147acb" }
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
            const string knownVersion = "104.0.1";
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
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
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
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
