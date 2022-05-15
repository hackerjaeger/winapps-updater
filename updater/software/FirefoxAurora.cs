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
        private const string currentVersion = "101.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/101.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "22cb295090189a87cdfaa3eed4dbfb9788e41eb12b5d8c72cc379586af1671ede4f6e477a22e673099a267f2b2c30d19fb41f6b885843a50f68bd10349643fe7" },
                { "af", "cec989b66b0be1f196bad23d74331a8d4e76c4bd320613813aff86d1dfb446d924c8d1259e436a7ac3f0865b80f95b8c9e8e3a9d9784370db9a5e98b29f61eab" },
                { "an", "0b5ce62a7b4d65c22b32c73d817315272459250b780f9785d4cc8b351793e45bb16e7310c86efc3ea54152d0d6ffa28ba9b901dd70662bc178146384145b366f" },
                { "ar", "5b3d9c5531824622982c6a6c47760ecb3ed50e907d9cd0da98c71a3988d1ff9b7c8150eef9c1c9d331bc5e7bd78740b4bedf92a0bb5404e8e696a4a541ca92e5" },
                { "ast", "c37ed801046f7086fdf4515c127035e9ff2440a8ad94eee0ffa74b78541ea1be340b5852349d2091630c1d3130ef42158bc357dfa97725a893d8b932b74f86a4" },
                { "az", "f4bef95a407c26307dfeeb68d11500fb7fdc06cd859bcdf4371c9d53406e9851a6ee610026a9f53239551d3b766303f916c8baed46cf9f0796b86e340ce38b97" },
                { "be", "e547e4fd40a45fdb49f455254c562ab7db341a28d4a6c8e31b558596a16c87415746a2957b8d92635de584acf074e805e60730595da84e4ccda314e1a0e94031" },
                { "bg", "cf867c8e13ee2147540744b0e8ab3d1f9571d45461d5cb4c2af8ae7b5298c61c882f087e6f011f77a33fb38a0a3af9d0f7d33188c4405aadf58c920f86ff154b" },
                { "bn", "19e6dd0b757a7b23884aaf303813fb05ae5e1c9b031c3971a113aef29f09be25c7edde46cead012eaa054f130169ff0f271ca229ad2651ab2931c9e060cce310" },
                { "br", "d4368eddcf4eaa427fa7d5d1861681b2c812b21e039f35097a7b3fa8d673a781a1e92ba944e30883903685d270e5fe1ef34d35777e87d352be7f0586f4717648" },
                { "bs", "1a77fecb9be52d088d4cd7fb8dbc2b721566432d28cf361be4a55eb7e46ef7108efa02057f04c00f7875835fadc352b6f9975b6ea1879397292d4e8f020ed17c" },
                { "ca", "c6e2ca2774a8891976824340fe6f56d0e570f0e3006eb19220b1c3f274cdcac53aced7e16c245f6f3c2c0ad6892d33da341aba507912b2fc0a4285915e225ff2" },
                { "cak", "c422f22e02df4a4f802aff21e94ba165759e618ee64d18081cd8c319aa669ff6758142a10f943d4187684b027fedbafa8f82b81c62e0e485442e976768507073" },
                { "cs", "8cacc2d4546b9566d362ce6c9b688bba8ff18545e27cc5e385131a2cbca14ad9d09ab2bee5d92e230fcb76e9aadcc41c1c3c6b72aa646e9d04a8906f9b920e4c" },
                { "cy", "8077eae4b7f732fd0c02948b829b0d8799c2cae74706b7f72112daf940a988d3a9e025f5be502de53490a803cf480745b43818c062c1d37f6736a3f24d69afa7" },
                { "da", "0e5f7ee0a44b761e195fe6dac5f88185f06c0cd88a6597dc913cab06dfc61cad3836b05d762c18b7a2cd759249329da90a67e60d9bd1b22fdffde8081f84d7d2" },
                { "de", "f9e69baa15e6e7fbf7647b90c80bcacfc51bd5b76d74f126c2701ade12bbd44db75cdf8d2d1174161a726e8614496868c725422b3bfcbf636427a7a5984dbd7a" },
                { "dsb", "22980d6ffe20e217f0f1569b5b8c67421816579551d02de83ff2084a2cac168022871b2c79bb9efb811ade428d58cfd25177783c73b4f4d37bd51a584efb51b0" },
                { "el", "ac2be42e39e37e6cbf94c5d8930639acaeb325870b2004bfcc94dca89797cea232a73778dd46651d8966947a3bcf355a7029b956016037a0e5e9fabc166ace14" },
                { "en-CA", "d7a46b92eced15b9c0a2de1ae8dabdee8082672dccfe0355275638838d1ab54992fd48bb67520d0d1f8fbbcb260bd9fedbbb79d4b816295cab503a720299bf45" },
                { "en-GB", "b97eefed53e21982189cfee0498aa0ede3c12513c6ee8a4a22b03f043d3c44e0dd22eee89261707f679a401d5d4b9a0199cc01b3ecc17c1133c5f0b988aadca0" },
                { "en-US", "0553fa1019044ac2d7eb3714d61e80e1cd1052ab5d9dfba712197e70c55fce26ac236f7f493de4e385e133c5b82e05d5d763e3a27d5fbf8d47f357ff03c6b0bc" },
                { "eo", "9170a0ae0eefd079429a178b7289e752f48d7161e854ee671b6ad3e37f69968c3c1dc17314508505da6d563b29cd27ed713699cf694933a72cd854e88e7ea60f" },
                { "es-AR", "276ae1e8702ce908f0bff188c6ad6f090275bce53c279539970685ffcd9dfd980d273938f9b627f0974ab03741993d56cf5f784e5ebecd7dc74942c6e61d7f2e" },
                { "es-CL", "d7e4631fa28c2557e910f899e8d943e3c4b7c4aacb6c55739d72a3a37742681ed0edc1950c35df822b66aa67add4174d6b6aa65cb7d1043d4639ce14ad609e0a" },
                { "es-ES", "4c8d686697ae53f4648716a51603978d888589e3d0505cdc7bd98c8980206f739fd6c16d0ba79814cdbdd5082fd78ff6bf18cb33bde6d69f56d146b2164a8c3f" },
                { "es-MX", "f6cbfbb6639c7708345799e2e09e1b0da6bd60a00fc884beb791ce2e50f960b75804d58288a6d7668205edbe76ae764261faa967887ee408c113a20da7de9ae3" },
                { "et", "411259682163ec0af09f11a4a0298d5985038d0e10af64f04b2f68064ecda1050f4256f59b62778335cbcb446d497d925626f793b9381e7c710f42383dc82f3d" },
                { "eu", "54009135f769f9a12631b5d953005a5a523978b7a21b9a22c48c6219c1cd7b80cd13075ff05f908d7db9a897fb7a723530ed045dc2f4b91c7e774db0688258d1" },
                { "fa", "d6f18f27e8d3349d1e5274b0c4b48c23e46f4c7601de3262dcc503b75c48512938e9e89c3053734d00271e95faf026d35e264e1553899d744acccd77776d8c9a" },
                { "ff", "fd4c832723b06bc3cd1f1895996467ec7650aab66cf45150c91e644c4d66b759ab4f464656365e56ebf1b4c81a723555e9eb00014df95727c172496e6d237425" },
                { "fi", "f9ad3357e5067e8f7d52a23a9bf8645c0d3e0d4772530f915b4f61a46d765c99b11482835b7a6e80eb3291c02a74535f70e62cdd73261d1a47b869aced2daf05" },
                { "fr", "986fe4b3542756d199439cc7ee479ef105360d4176f80f384d81dd7d22e5e42316996693ffa8ee189491abed8d81dd4dceff79cca612e2f70595e112628cbc29" },
                { "fy-NL", "2820bbfa51abe680664ae27e3eb8fe4d20887907fa791a4907bf6bc0904864c8d7dd08825ba0ea45b157f622574e0c529e1f419e97dd4058653b8e0271b37d3a" },
                { "ga-IE", "3e7a6580275ae0fb6b5f497c123199d3ee05698b4b643a19cd0ae66ba890375fb3377b1a33bd7c69a816966f2fdcb352ce439953c8057bc1bcbed42e85971c61" },
                { "gd", "fe56b6c265a56cc6f7a0005f4eb07f4daec76ad3e05bce28a49873b3204bb8cce8781e948bf992ebabca41616b4ebf8d135fde129391081126ce78cdd7e1306b" },
                { "gl", "fede4509cabb31bfed63d0da088a246d3ccd8a462a7292ba90240924f3c0a10a07e79840eb9fc96fc0bddc6379281a85496b453ebb9d48a0edcc5d28ed060119" },
                { "gn", "d507d9032274927a951a75a29f4730b8a6fbf15b000c92488ac8521aacaa03a55f468e1f5b0813dedd66cfb14de3a69492b8a5546089200ac06143ec228458ac" },
                { "gu-IN", "547e6c64f906973c1bd8822f45646b479b4085cc1fc1c972353dd7411b31044cbcf6b5ce2d5d5ff0fa3b78cf1fc81c28a44dc24f05408aca38a4c93e4ea1ed1a" },
                { "he", "bedfe396eb636a7c06427c4aca6cb0778fe829eb985a5832d2976cdceccfa00c7f10a1c2ff7dd33aef661f67741ef901f1980c1cd351bb34c8ca80a022555676" },
                { "hi-IN", "8612a99553537ef2a8c8830d1404cb7fdca422ef9391fa70c264e78c2875c5333dc1e605a3e90a9e4338cd359b339cad776af8fb29b920a23777229db048f733" },
                { "hr", "8b2f4cd44d6e465fcddbad9d95722af27ede55c185c3705abc5dbea5609b19d54fd775a8d402a3c54186725e6993a6f3f6901a9b70a00d6a2501b9abe2c118be" },
                { "hsb", "3cdd5cad7a05ac270ae0e0565501ccb03f0ef82c41d0d399e5a8716b2998d646adf8138fc32b880f403eb694a7670288d9eb2b5a3ebc53ace06a86a67cf214da" },
                { "hu", "0b5868f81ab346e273a2df7a2109412d94fe4a95d3e4040ffa80482c783e405febb4be0a60be550be0a2c5c2272986e477e784d812c69fb2c4b5cb6d9c6011ea" },
                { "hy-AM", "ff891158833f8273f95b4cfe82900ef0f963d76a890f45085e57103ef7b9f84ac231b496cc183e06cf12523e8f87e6cf2c203eb19eaa7df968eb602b66c78adc" },
                { "ia", "51a027bded6ac37088ae48c5acd7d8b1b3fb39455af643a39d5fa4bc04a735a48aacb00b50f98a6d5353bf58016d48340343910881ccc9f2a666938ec65d0637" },
                { "id", "c97bd1d2d3a035801fcf04620b8ff79f0be22c69401b21aa6a794d1c0497db205424a11ac8aa4ccd1e7142e15a12fd3bb1adfd6f2f45e998bc3123795813e4bd" },
                { "is", "566ac2c7254592ccd5d546c101f4e19c5bacb1d577696e66dc4436ac33c921ccbf4c84bd205ba69981f70447f6e12c549b89b1e6f4fc27958f66cdcfe741d828" },
                { "it", "7fa78b1d2e679637c4c86926e1208c303fc2c61a945635a2b8df88db356c03425846fb8174ef284fcc8a9c40c69105eb7bfaef14d228fad6b6fc9ed8895eead6" },
                { "ja", "e7a2cd7806211fd85e27beab3a9b86b78f6a9005ccee1d47665cbe18a43d124cb34c91fe3e3ee3fb3bec22bf02013f5bf6e304ed65378344e978bfd4fef98802" },
                { "ka", "66549dbc6634a762143df0a3ad4476f4aae3fb3f5deb589fae3b6fe5eae1c9cb478a411fdc258ff1f3023af5025dd1924a2af80f9907adabd8ba1a60ff1cf98e" },
                { "kab", "057579003f17bf28c0f9f2d15afbd722aee3f212cec372a43600c4e5894b9c11834a2f82fb65630cea1328b16f94c455b65c28a3d8decca53503bfded2052c95" },
                { "kk", "756ed8e873a5b41a2410088b73a68f26af487aec82266931d42ed4f70af4713d2b31d6996d4d6527b71e3b95137802bf22fbae4413c7af98ce380c52b31d0ace" },
                { "km", "b62a722a6489ef11350d077f668f885c36458abf153f7532dff33f746ae7e345876976a731c78d1ac506d75f16fff3a336a9e7cc59106862e820f04ccd84d5e2" },
                { "kn", "915cdccba2f1fcef619534dae6b82c52bc0bf9465875d04ba23d1cb132e6de45369ad16f2b8b4a4ba78af146f161794de79f4981a109c16065bf578ebaa64a52" },
                { "ko", "3c983b325f7401a97bd48bb52d4ccdf1577a1d6284d2ea69ec591c36c3951943ee63a5f5968f0e8a7474cd31743b4bfe25cb0ffb899b6d0a92ec7ec47ebdf39b" },
                { "lij", "0a8e8bf44343acaf55d26fe6041ada9a76c8c06c3308e62fcb652e19a6977381f8010f5c54bf03eb0b2c7c45a743602f8409fa60cdeaa3dee9bec39b8d03baa1" },
                { "lt", "d34560b065bda790a8f35aeb93ddccd66dc269fad5b16aaf7f7aa9baf024ea7c109df3e7605dfbcf32a14fb58558ed165e1971db6ab2b5642fe907f9d5371ed2" },
                { "lv", "9a1cbe508b4c5da3c8c3316340fc76e09a330c754fbd5675b29a968f5d1c3cecd3c84a7000b072eaaa163a3ede4b56ee401d079a1be043d144201ef4afc7c829" },
                { "mk", "1022c7d1b08fb223dbaa58fab329f7d3ebf05e92a0f6af3344715ed31d03e45cd82fdb0e5f48ad3e6d81f27b9996ce9984e7320f40bbdf29f828d0926ad3e2bd" },
                { "mr", "0e48f6133584322642eb90e4f3199ac481fe960c007cdd1d5e3d261856cfe4e7c89f3f6b86a3b3c4aa432a0fda226fda19ccd50465bfabeb670de37e79d42308" },
                { "ms", "288c3ffbda71511f926bc8279e1bb0a5bdcd0bae6b2864779b3deed706d2fd6721e69a74643985c438424251aea2486b9072c6dfac38938bff4165e72261bd75" },
                { "my", "f1ef7040e9ea208cf7143b1635d15d36e8b5e5dcbbe2fd128b41dd9ed01b7633d346f674b2c9b393487f349e31bb4ea322a2d94c58893caa62b6c450e85b6e7a" },
                { "nb-NO", "bc20776d6defce52c289e77809f3ef9a4d3f44c665a4feb368c38969328881b267f782bb160e5019785b84ab49bec2a684d15a935ff1f59ddde68ee2ddb2148a" },
                { "ne-NP", "52b7cdf5e5b09b41f484114b285b43106e7e30ac3d51acfc4335b72a2d0c4aece3093140270d22e9eff0f0c518679c96d899f519c9510afb02bae042926143c3" },
                { "nl", "95282e03a7b707bd5af20c538481c1670a3898bf267da8dcd4c3de40ac10927be8ead98bc0401930e83df363da701c6e98a3706feb9c638fe37b9fe10ee77326" },
                { "nn-NO", "c9ed7767595474350ea68b5a7acbfed9a3930397a957ce8309b23e2138cf2a80cd830cfc059e20137e086340c4240cd9109c0999f777757a0791db58669d75c5" },
                { "oc", "69b30fdddbd54460e78ec4cf2db8fb0450705ab3cc956e8d300f2079538afc3ebbb69b2f863608df8ee0e54fcc587e7bec86f432fdbffae99c9f81874b957491" },
                { "pa-IN", "602b8ae34878d05b4fdf1f15b9678cff7d5e2f4b3f228fc810df8474f54c5a943bcc90aeffe651f96ecf1509a52fe0d4f25c1d20e26bf09e620093059e1c5ab1" },
                { "pl", "a0c0526160744aedf16bc75243099b5cf0b212c5e947458a1a8947733ff271e22aacd1781c57eee04c22c29cf4a6cc0146734d4dc47bb70ad11fb9253f59beca" },
                { "pt-BR", "095fb5006746fbfd63901e4d39750d43d3562836ee2e34a136dbcbe7ce67956c9cf87fdd6a049350923549bed1f2dc6eaaff315425d27bae6a187ecaec35fea6" },
                { "pt-PT", "29da64147f4020d09c2eb87f87a1436869e2887b66fdea4674114be1faa4423e5c47cecd607d709f2583fe6933b77b54b8ea43e9a39674e1219cf0179c6758af" },
                { "rm", "08af5c65b191294f8a77c816b1bb32a02144fa772edb27311f5324bd8a6e649e9aadf341eff65353d568b459f8761b55db2eefd16ef5b67c460c510c502fa413" },
                { "ro", "0368bc28ca847fb308785f3efcc2b2655ce5f1c65d5b8a638af7f6f1e58be199d17a0064e7157d37adc59fd65ab9ebdd2d95de88f8cb27fed752cb708d3a17ca" },
                { "ru", "7f56df3cbfaeb9c87e70529a9d3101a6dd96db9d2b2a4fbcf862707685dc6e7d8f6139783e71467f54d5f64edac5df92af92e6269abea4b2dcf0e99323dff023" },
                { "sco", "0918da10ddf62ddbd43c9c27ea6b6ff2cf1afaf0029a07c730871cbafce5d6c4ffc31fa766a64743af2a8c43d40f461c746b56ae9a9a18b8f250691d31e19b81" },
                { "si", "e6d278a3894a55cc4ff8b14bab6708725c72817c143570a3bad970236bec94af1729f65082a4c6c497b268ec9135d11635040537d14af7ef2e70e3098d217be0" },
                { "sk", "f96ffb6798bba754ced5457be730c2ea9d00681b868f323967155bad5a0c08b50037efae0ae04508f48c6b964483b34b7af470a3095e0f06827e006d4420faea" },
                { "sl", "1513e409ead5c6361673c0ec24176a72f82854bf744386e066ef2d3376b20cff6d03612f8deabca6b7f78ad227ce4b81070e642507bdd9b7ac0ee04e0796f1a5" },
                { "son", "40af93ce721f387cc145ff6dee48097b695b700aecef4f06bbe4954ab0dbb216a2287a88fc3d061b4d2a410bbba550b3bfcae46a3dbdc78226ff860dc48de6ce" },
                { "sq", "35409350545b2aee479a9b836c811d1bbc55305a6ba08727be16e470c9144220271fe3a3d62872247cc1c630d086176b053cd18c98c74cc7c23120937c69bb08" },
                { "sr", "e419c5aad7dfb3f9efaec982da8e3baac3d6da3483269eaf88791eef6e19332db4080429173e85971708834f07c5903aae09a60542dc4aec5383705dbb8289c9" },
                { "sv-SE", "c078ac6d736cae8e613dcce76fe21ef6f2adc7ba9ea69b36e64d3df26f86b6c33580274b42692a39c4eccc2e437154f743810371029fc9f235ea871f444d0f96" },
                { "szl", "6e1b804df1a469302455176f736c83008ab5639ef346886dcdfe9ed5a2e1e3fd99c91e9824d844c73bdf5501f0ba197c5b143eb6c45946be5365e4ff7d226afb" },
                { "ta", "da02dbdc492dd594fe24ddc29da98860f4945fef8efaca83f6daeca9de2a9cfd91b276ad9d6c64e9220400bc0432818d90e184cf768c37be7cee75f2b06d9f94" },
                { "te", "3339618bc6382fcac3ae7ff09536393ca62d865496ef61ef7ea82ded665f707f70b907e4af19ed64eff88fe13dd967a6c4dfb7f554b441e82d72d12592f6b661" },
                { "th", "d35ce7702d21252bc11508e4f26e097143a380544ce4942b8367c2afe4de7abbeb0a854b34652592df8888d8162ce1b4ccf60ecdefea405ba3285704784265b3" },
                { "tl", "a1e427443fd3d3d9dea7a940aca7e1500d72a854436d01a7ce63f3c48e0e3b7efec66d71501517a5252fac0195a97511042e254e11f5a6f9f8140381e9df9e6c" },
                { "tr", "0b2cdb86470ae8278acc2ff9183f06f6d21b56033fc9d5ef78028f8ff74517737ff10cc44e0407242ec37958cda16317bf99ce46ed393b164223c64cfb3fb99f" },
                { "trs", "9b6bfd5127afd820462122cef6d0a862cfa81595d2912ff6765d8d69ee288d13e2482805c84b0ddc3597ca0872d1a50ef0e22c7eddadbaa952d1d1836f2c5f55" },
                { "uk", "f5108c51b889b5a141072c5057cb2404279911b979d721de3654c884db1ba7819570fd59d9c0ac4198d3dff0820fb1def34ad620504440542b145fc315b71fb7" },
                { "ur", "48c1b53e06dd8179217d9a695ec8c115c5cbaa4e9184a3363d1dbf565713e3316fe0f82170e2a779874538fe858df8a3c9e2f121533f1655925e51da66658d91" },
                { "uz", "c3df8b9a7a1a2933ef5a1a0892e0fc7b084489c33d1bdb3aa13c98787fcb19386a123264c906b5bf42d05983865a37445a7f6b92025c41d86f39c165eeec0ce3" },
                { "vi", "29a28ad02bf0b053eadbbde310c18edd269581bc2f3c543cb3f3383c85034244c47b84415aeb0eac58eac72ec2c1eb7725048021846c76eaf2c5f221a6fffbed" },
                { "xh", "e179608a292fef4d56af2053438933ab6c45a95841ed0c72a461d85305ce1e0e126de971bc83cae87e8044dc39f944918f922eebbf731dd33703a7a10e92bb2b" },
                { "zh-CN", "ea55107bfb19899cd5100d459358c05ddeed616a15f99d5453249db3bafb925aba4a088019712af50fe84fb9e3de764ed4a66c2e0833cd0439a823fe29fa7e1d" },
                { "zh-TW", "c09871f5542d00108a8483e1f8bdf74ff1586f6340693aa44b5935a1eff5bf0fd483f2c8e71e28539c60c002ee27e81a21eea4eaade24a7d385c3580a4704e18" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/101.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c5d59c2ccb463f418aadf05045fb091a6a6aaca01878d2a62ac8150534dab183c55c20d2ad505e481298c4a8c35e609f057e090977a67a2c44ad10ed4c7f97e7" },
                { "af", "967321ea6d798dc63dcffb7552a2199f4d23396bfc565b4d4bf1e5a2e8932e64cad36d3314965e548cb36e03aefc94e357cf378ee12c59fb34f24cac52ff5408" },
                { "an", "53349298552a92879e0ede58167d73ad9d8a87aee813f1bbae1c620304eaa06d6ebd66c8952180884aa25e8a3733516a0c47094e09c73271431adea3198d49a4" },
                { "ar", "9ebd8edfa757c11ea207cee9d21dc201f18ab05abce5a2dc4586733e6c590853dc530d3afa2801ab18d351516c4d3de6026a33fa8adea8c25f035c51ce1de915" },
                { "ast", "de1e324efa642f950183a9970209ed6771539efa6dbc7f37b833bb915b79d0ffea0573d631cbe2ce79d04006d3f24ad8261ad69be06325e9b9a51fe5817436ce" },
                { "az", "13b58ccd9c421febaff1cf028da98d9fd4e1633e35303692e40b79e5768de05506056408603f45ff3d12bff00e8965e77107801acdfa43b633d0237b623e1a21" },
                { "be", "20a3b84c5e1b67557303a05c3a86c5ec1d9bb70ac6cb96bcc35d44faebd756b8d370559cdfa2e785d4149428ea5fca64bd4b4a869f32542d88155f1e6d92d9a1" },
                { "bg", "b285af65f7606a48738cffb7bcb6ea87a0a75c4941d91bd41c5f90eeb4e111cba2c69989bd2259ccd5292a4dc11c0ab62c7b2e19ad99f73b46d521733459714f" },
                { "bn", "a047376946c95ab0551f165408bbfa227e8ba3abd4e99f8e7c6c8683767c17a75ba9507bcfa33ff0504c13a2bd3389547556f2d76835ff878177fbea987f0c48" },
                { "br", "19cbb839a60c4570cf65f870f699cbd4db896267c5bc1d81c19298647d8f7bb7d86009bbb4430523f2ccc9ffb2462d8b8688b8c32c847b9aff121fb6fd79ce0c" },
                { "bs", "acb89f385c0928160c8980217231770a1ed511b3add8b07e6d54359c7b798182094ba7397d99bd2bfb47d71c18d5badb6b8f57b55f7b5b157d7cc41d996b8aed" },
                { "ca", "ee91d3ea2592900da169cc8aba6ad6055d59fcb1623fec68755b45a23561f823b8fb5a574eccc5194b0b36a19525a0f03a56b516506d78fbe7059e402f135b8c" },
                { "cak", "e9a112efa16e75613b2bebdecc8943770d7eff1b37702dfb002ee0cf7ce033a9c5c7fe3554f33a908c76628d87f599c4376d2aef89fbf72f59f2df0f47168e02" },
                { "cs", "232cf9d9d43432b79d2950289afdec79222b7171efbfad91c578f4450f0f3b11f9b5deb2ff10ca928e9642d65d582a9c1238e01080dfd6192fe5cb949bd709b7" },
                { "cy", "9129a15e1534dec669dd80cfe4dc155d96bd4e673e1a76c12d3e78df8324363fe84ea8f73361bfa476e3e7cc0b26d2bc94685c5164facd32434565978db12472" },
                { "da", "22e54a9c0c92de5ac928e10e9a0559831abe5c050e1aaa3f8673d627ce86aae6cc60164c65ece11134aa203b78167a7dee884191840836561c052a378a0a8baa" },
                { "de", "eb16d865426c7f982aa95f410ba03a63d4550e6dc23e1c76e10c43dbc8a533b3d7907f062761c00ba2b058417ed6e0de15f6e77713e8e8f8f0fb44e363d7284c" },
                { "dsb", "87fe4daba89baebaeb17b09571373d1651891886a18700d2e8457f6570bb0cedd3d4e3ce67bbb2e2ff5de5c1e24538e50769530738a613d218da06aaa6e09dbd" },
                { "el", "5864ac9426f01490ee28b807538e047f986e4f1972538236b58fa82dc113319de58635eeef38e9db6370cdd3fb888987a580d3c66de48afdca06cd740de5d4c4" },
                { "en-CA", "9945603ee400683c20f3d75b1cb6dac197e50918d617a96c5a08909736a415271ac999409f5854eb9bb2aa07ac8f5d38be12269de1008d39e8a3276a91a9e914" },
                { "en-GB", "5174abc50807f2ea63905f3dd27b114a7f295fdaa076bae853911ddc959e6a4309dd4d173aba61b9f488cb995dd96a4829ab698ff1a84707d17b290c92d8cf6b" },
                { "en-US", "8ce7088283889e9df706c6537214d0904189778cdb4a617f394d73e7f63a80676799a3fffdd811814c04347a84791ae27e68499bfa6cf46de0360cf164e7cf38" },
                { "eo", "a3ecf27bb55ef038c1189f0792c5c9acdfcf348b29182961ce1045b837dae00399b4bad4376c4e27e8d348cbf21e3f210f35e1ea2148a6dd7385c1540053dc51" },
                { "es-AR", "95f160224aa16c1dc311824c9a18b3784124bb20f231f89a360735de147bc1a13d3bbd244d57133dd2e70ea22388147fe6f41538641517f783fee1b023925ffc" },
                { "es-CL", "97159a0b1b5c1876c46a6a17d0b718ca86f7f2c3857c27730d2b6e2ce009be30661ab2af47d3d5222fbdc9033cce708396491f0a01069d3d4fa52f0685f72abb" },
                { "es-ES", "347ef7af97cf6ac4dbb5a982d70a9a88861d12441b97c1676badefffa3bc6478026ff61a14ad87066a8894d1acdec187e71fd9f4a28d4539e67d0cc4bba92c73" },
                { "es-MX", "6d07a50ea66cb4f395be854447b858ba31a48bef902b7957064a64d3ece84c27cb3745692cae3a1f1b88c214c3fa758a6801829058dfcfb76052ff534c047cee" },
                { "et", "d05ee15c59d3b0bdc77d74bff631c22ad94e85bd425cdd3f89a201ab1b8edf7ba0c40cca02fe6d38a24f290879b2cbee4c0114058bee0853f883127b2560d992" },
                { "eu", "ebda1130b6730d7e3d302fbf0044e3312dd6af8757b83e7f017d410df0e1428e2c749b37e91ffba1c3372a289fabd27d5dbe7871ae2ab15f4df068cae22fe364" },
                { "fa", "488bba020a89b5c7cbd20b36f92bc43cb6276fb0328dc887c0c802c447be325186c54ec6d631d9d81031d3df8529036b6ac1b82cf95ceeab559b2338b34e6100" },
                { "ff", "75bff7d62054339846207a9d05557550f3a35858bcd675e9b54c422ab141ffd6777616d42048f24acd1d5b413d83abe386e0978d3ca15b3dc36d8db43c56562c" },
                { "fi", "14b8905a4e4bde11391c4ca820ff44cfc03b5c900d01b33bc3d2f60e3f31918cb4133ba9cb7b44020851f5534b30e4d01316d50898faa3bc0fc2c1f29c761b7d" },
                { "fr", "dd9acd81e124f5a472f181a36848d0ad5b438626fa27dd77af5c5993195fe498ae6e4b896a05d56d019db364c1bccc777eacd26cc80f7da4bc68cff444100d3b" },
                { "fy-NL", "1b35792c1ed1bd5e8a6e9cdf1a4afd9f8d200f1c11c9608c564ed8fb5f2ff54931c8e52913ea2495e2097eb1c3482eb6f5de75747e423c98dc3d53d26c92e373" },
                { "ga-IE", "2f6d05340732c75ff0653160acbb5640e391bc1fe42025668a8e0401b849e1accd24661b56fa08050440d9bd7db3aa5ac540fd4685b514d7174eeeb5a0949cc0" },
                { "gd", "00a6526cafd9e70071e36dc730b48c185ddda5b53821f5ded07d2fc61506625903424a531343edd02a2ba9302bcf90ed72becd7d315ec5802a6eb5f19f15cd9a" },
                { "gl", "e5d404726e811f75fc951a67cd63c8bffde6649304c6c975d000a8ff93a05b2aae59bae10f25d5c90daf637001d53a574a3b024b102f4f004d349084a9e1c60e" },
                { "gn", "470180a303cf57b7a74cc02585a5b4c3babe4c120ce27cffa735938993e75ff1d45799f0fe70496774eb7af0eaecb00cf7e284155c0ca9044a49dd45a44eca16" },
                { "gu-IN", "d65ac1f1764bfd2308fb10cb507c77a42c0b0347de484106ec407eda244ef4d177d2489a80ef8accec8990f4a4e961d33ea9bccb9f258e9306a5aa330992c6c6" },
                { "he", "f0cba1a70ec2dfb9e4ceaa855e42f27cbe265f1ababb86fc3b9f2fe3677a3915da651327361ea9065efd9b8de66ed31e48e339c546f543a49485171890ef0d85" },
                { "hi-IN", "c2935e1c9f6405cc186ecfeac1ac56dd342d09d1ea92f7024a2684d3f94e34749f56fb9fb7c97625c155ab9933c1b737b6dea686ff7c0d5e795ba08ae435ba90" },
                { "hr", "f332bbb9f79e79ed558ed6de622093c746b058ba4c7c67a4809f02a68948707be58acdc5530703d3f65af0b3e884c6b93bf8453fd6658370394dbcd8caadf6b6" },
                { "hsb", "5435a5b0723aa3dd84f810ff25203fc72f48bc0efe7830d7a5fe32f0bcfd674cbbcea0f59f1443abb76deaca723f662da63a54ea3c44b989ebba9f0aa71d9c09" },
                { "hu", "6bc2b2c550778f0eb827523890a6260e78d8eb442e53db9a22612e94fb0b1b87f02fe8347286a81d531b0457aabbdb4aa195af54ba5efeb6aa440ae449ff11ae" },
                { "hy-AM", "554591bedecc63b145593b941d66f65f1206a6ed9397646da5ab2612ace2ff9b5622f6e8f14a93365c57d7412796e39e5cb3ff9e723fa44845de31358d273833" },
                { "ia", "e084e4fc5b12e949532d39fc2556905efa6e7c4b643d041f51082571bde400ac62e13998ea3bbc3a59d67b0f9d90c90511cd6489786b62d326f2e267521963f9" },
                { "id", "2b15e9602ef0d9cf5422a3ed38064bf6a2469af737f8464814d48b3abf44598887a9c1fb3f1413e8fb32405fbeb5324629a11b9d575f656af2272d7e46aae40b" },
                { "is", "ab57ff3efe1c202a8695df0f012e5f30684159c5cbe4754cda5271ac46375ce5fe51187879c76c8c85beec377e9bbd9a5185c90f0f0b73230b0bcb1aec668941" },
                { "it", "0ab18abdf8019cfb01252edfd4c26f4d54ad53a916cfe9c3223af7b3213f5a5cd812903f64b881a429e4aacac7cf4887f33b4132349c2868f91d7d0053a9c98c" },
                { "ja", "2ea9ea62973868cb3298ab9335ca30d25fb2bf4e1dad52a7a0a48cae19d3946b7be5be6789bc7a8ddb48cdb8cb245cc6bef74d1d989585d5c078f6f5a6cdd0d0" },
                { "ka", "e6bed1290c14e3e90a5cab6cabec82677d90c1140d770c70b4c67aae18d25c45e3a3cc268998998835d876308f6a47c9557657a734703e6079d65243fe000c2a" },
                { "kab", "2a56f25f6b8e0cd6c288c47f2d363139d03c185b7cc0a2b873fbce3ba1a1bc092ad4e9a7511bf636a4676c67edee691ad944e667cf156a7ae70367cbea9a2249" },
                { "kk", "cd746456cf1fa6afdac5af7ce8b6de7d7ce9b642e1b811c623535cd00d4f914e0cc30be482ba950695d194e96f18f0115fcf224cfb025c7264d6120c27aa1182" },
                { "km", "d8631dda5c005763ebed57954ee35c06b06dac091d640f2ccf1f3e3b0006dea8ce2d995421a3906dca127b2e72cb8a5936161528694b1de35a3b41208ce45169" },
                { "kn", "3113fb1b62c989e5519f1d219d1e1059996a703f00e4f0716044200c4e0c3f3894aaf78937d64aa48895238bbd4455483e87e13d6102c6c9e103b1d328121d88" },
                { "ko", "ce7559afdc21588e25b6de9863afb4650c37f12be358dc4fa813dd4a87810527367efe026ced97c14b2ff7f86b911be4ec107fa4b53b97d247118f274b142d19" },
                { "lij", "cad66842fab11957bdfd090ffa6c1e1c98808c2624dd4a85b7071987aa74567018b6444232ab2a086cfd6e2dc24889189af34cea7cfa0e52362c83039e2ed80a" },
                { "lt", "b9a5d36ca8a1cd0af6dcc77120d0d38c5e0df3a857ec6ee4dfe32aad7aabc80b096c60ef3bfc406a2f0bf45c5475350369a9bfbc4c8d59a1427abff2edd9ec95" },
                { "lv", "7b80232218f573c20e0f023e295ae197c72c489b3c02a73e13f0803f51a6fecef177c0e4e865e60a7e961050619d361fe383873f7826c6671a86c9cd518715e6" },
                { "mk", "ba8350e1a7a0556d903ca0187b367f58d062643f5cd5444546c0a65e3ba191d418f23541c4d17b81a866ac68df582c68886062fa1f08f1469c0a2385287c57e7" },
                { "mr", "62db32c607cba9341d23af7eb0cb78ecb7a5d5b1da1189aa438c56cce23cedb7fd9ae882047e98d6fb62e880af507990ba5c0c2375b25537bdd5c251c36328c3" },
                { "ms", "eaa409a5882698e5d604cb5a4763e655cba90940e13969698439fed6ca5211a4c48640d24cd13a5435ec5677f703233e6daa1275f0fabd11243be1b5593d75a1" },
                { "my", "4002f63ba4307137c3530048de6c4a378503dd7718edab70776e942988677e6a2bf3469ae7896651531bd6fd6d649992293c520235fc6bb15fc1a813d3d4d461" },
                { "nb-NO", "1d32d45a6e0fb522ff5763cfdd485332347171b694fe6cce1348c0d9659a013b4654bd746b18756e6e5505ba60ae90332b9be7779edd37fc11fac3117649b957" },
                { "ne-NP", "400930c874d5f7503ebddd73ec7d3d652bd7b397312b756c1c795a5902968fad5f1f0d234b6587aa8e0caec2d6c4130d2365c959b997c74b9c04016db6985fbd" },
                { "nl", "5ac401ebc7454a42b8417e8151cd293a8acf51f54136b4c70da36651b41a149cd3b996527ab1cbb9a902b0bfaa71beab55c672aeab0c68178841c2a6132f4841" },
                { "nn-NO", "9593ff16cee8755127d2ebd0e76b8976d23142fcdde5769f47c391b7179ffe315ef38aa94560d1f7b64c19c1989d6178748905d91c8ac2c1edacff34e312876c" },
                { "oc", "30ad5fe9f2f7490747082d485a0285988e57ba4de7c33e8f5b024383d68802fef936568c3d3f5b78b7e10fc2939e57ec5b3693f5d60e2ec0458bea272b877efb" },
                { "pa-IN", "f5561150132116d4f535a4ccd92e2d59cf1f54cc81ed2afde9a06906dbea87bf3d5a97cb97537712e754b1cb1ae9dfdf431ad690982ef48bcd775401adc62657" },
                { "pl", "9d38447014e1fead761be0943d038aa4aa034aa3d9f00ef275913b458184b2532a4fe3a3d653a35b2aac02577117fa9b648bd61539a270a8c249a5f2d1458851" },
                { "pt-BR", "bd9c482215bdcc31992f27962e2f1c7be70e25c7997713d803c0eb59d130edeb0a0135480d8bac48a9d52285b9d168590823c7261fd3d69fca374cea0bb1b8a5" },
                { "pt-PT", "e7f023e354302f705089b74a64195101e474a2cbd0973b7487336899c7ce9a73a9a44a16f850e25545bf6eba2e77d6e8705ec8ed80db4a824e047a2b9e0a877d" },
                { "rm", "a9f570bc75514f1c9263b7646976f38783661e78540d556747dba6edb5d0b5faca35ee990cde39ac74f5d21643d77049835a3cfe4426bb7c46e679f0feef28e0" },
                { "ro", "ecdfda7705af66746c4a8bdb305f655f6324ba017766b25e94a31bf0b432b3d15c2b0807457e21e6ad97144df57863e84c27e1fea1551099a8e7251047691688" },
                { "ru", "84168c6bf538f8675bc0f9ad223c3a359a59d7cd4a8b80efab825f9bb899679596e2c1c8f7c1612dfdb49658a4d54effb312a80c5c4633995ed4585a743969d3" },
                { "sco", "c390d6dba3030eeab3d5dd806fb7df93f5c3a1c942eda44bac58f72e9b20a898e4515566e943a887ce4e89353fc619600ae5d505d93120ba0612cf705452a5e1" },
                { "si", "77d1a08d3b432030c28c673562bbe262bce18f6a5eb41a5a5bc2691c6438c03098a48d0dd4c143ab21ea7355a5a1449752a5179b7befc0473cb46e63c8b6ebc2" },
                { "sk", "5b01c58982effd6f48fc793cb7448f0a41aa52e85e93fabc3f3c534fd0bf7b6fd7b548b508eae3358d931596fa8345d63860add62f2f1233aaccc94607bcfc47" },
                { "sl", "7bb8b4e7cd9d863bec9808143c9d0fccd95a6f5b7fdc62fbf37f235f0be4b8d937ad9a23c91dc5068aa124f94a7b6328a83db36f7596ac803c2e0700e4f9b663" },
                { "son", "12e8de40975006878c22990ff126e0b8615abdb136c900140f7c1536127fffbd898ca6063c41dc529a945b0d6f82bfc85df44d82a6ebc9d5f890f84a348f0a8b" },
                { "sq", "0351c5e02d2a6f3b21152bea357e67d788fdb3e9213b5fe018fb44fe0da2a83b04fb4727459428f5b24156b90a934977de0dadf005e67d09ab812e92bba9085b" },
                { "sr", "516b48e8b24b1c9e0046cb2e2ff2671ce0dd2e553e7676c248c7101201d186ef20b148a8772c41eb05a09a5312447d20a516af43cd9f3b3dbb1f2def41d28db1" },
                { "sv-SE", "6fe44be238d4ca8ec78d021a23839ba4974e0f418af9eaf391cd4651f1a139f4f0a8c5600268cd92df34bee01e88c406a9aa085fd95f34d0aeb9ef7ca78e767b" },
                { "szl", "60756169086c41b56389619d84b41fdb1f9323cd12f0c817f2eef7f84a5a2ce0b5744cd0e7cc9049009b6ecfe5e21a04848ae9f1fcea0ae31daaa33b70988132" },
                { "ta", "418649205c710fb34008262d7ef9c44adbf8ac1cd57e1d540867ea92688c4621354ef4f02df58eeae4ff1d0199b16f8f0cdc09164c1acf64274b5fe5ef36d2bf" },
                { "te", "732104735b3e9dafce949539e562cf3119f6c86b1293ce7e27cb4a7e818111b4b796c6c7f6c9e3c044e868569d41ff047fd870c1a08cf1df681d30864baa18b8" },
                { "th", "70703ce0cb00bd03c3a43f9ecc1cb9a23d63a8f0ad4e5c485c2e8c5ce053c1114d02164d91405538dae7537e850a6122014a407a4bdd4d67bf8176f8e3e6c3be" },
                { "tl", "223ece8032b7a3ce1d04702fbfb05be6d49d75c58c9ff32075e754568d28bfd3921de086e061d05194dd86d36e521b1271bbfc836d0fe5c19119d72c5f283e22" },
                { "tr", "ec6079ad33dbaf86dcf84d3158bab5b94c43888d2359560c1770b0d2b1eca0a97a99b95e06f1af7cae70c2fa4c0fa707832c7acb064de894f5608a47b2cfe063" },
                { "trs", "395e8365ea012d1713560fd17b1f433545f8ef0a8983b2c46f7b8863c9c6cd2cb756239eff396d2c3eda8c31ffc4f3a5ad6c2ac5ba3a5042a868563aafb29ab5" },
                { "uk", "50234fa6b86c54f55a2dacbe48a9cf4b1247edbbe71c3c2c901962213eb61819db474bb4fdd04508793224e7cffe4a74e877cabfba1dac59647d92a307295c28" },
                { "ur", "8ef29f9978235abecc8b02d466005b0214e430eff257f12957d40ee52377a95d3a99157eee09970df949384fcaa5d591354ee3ff159815d274f794caaafcb628" },
                { "uz", "6fe6f6908f820104bc215d886225e460201a6423a7e88291713301308859394f3e59b7bcea59d2a86b41a85b48d017246870ca2ef6feb3fb9e1aada06d6b1b59" },
                { "vi", "128fadec4c639f404d1f7bbecc89a0f989f72f269e4752eb8e7ee687dd91053325ec73918ebe4e48f1d0007260a59830210816596f90d43bcdeb34d9ddd18697" },
                { "xh", "aff64bc21b555467e9efdabfb80a54de10fb1614ae06778cc8f2d2e093feb500b5717c6264a20fdf05f9484b7855e5595f184d7adb6714961f5b3558efa45b1f" },
                { "zh-CN", "53ba37ca8f80ff7eda0f09f07895235f08647106700d3e63627164a786608a35661b0891374a59eb96a52586953864abb379da5fce32ea385659abe244f0e5ca" },
                { "zh-TW", "3ef4bd94b56c4088217e003b19a72182939f778989973998b0066addf4b43767ac5b6c336f4dec3616e1415a22e3277135824df6ee48ce8b549fc75ade1aa557" }
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
