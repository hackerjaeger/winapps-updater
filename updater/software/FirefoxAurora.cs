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
        private const string currentVersion = "108.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/108.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "12586533977a8cb288050e80600dbcc5b0ed31184d13300efcdba22770af25b53f44474cead5e86648ef89e5bae172394fd1b8da4d28a330d617205a43632fb0" },
                { "af", "678a541233be46e378bf0bf6b08cfff233e2458197f4679bb714083e4dc690f508c6796ae84c9e3babbd4bde7eb3e8a450454e6053a59ba1dc7077806bc2c7d0" },
                { "an", "755670f9220250a664476a673722e39d8b259038b623cc0f0f0f9baff14231de0ed188cac61076ad5bed0ee926e5499b6ccac97f7ebf5ea54b4276c6e8ecbd5f" },
                { "ar", "c1c6d51f26a6eba11d6af00baf57b31badef1c68ae02e0d09fa76790e2172bf1285e87e57668151a85de7a37987e0ebd5465ccc93efe722e0e8943ca430d9fda" },
                { "ast", "5d878dcf882763ed250b32f6bc97eb230e51a55319b743ea51d23c9e8fcd1da5c393a8a85a902a81b7c42fdaa508c10b9b1e83773e892b377f9c6c7669c426a8" },
                { "az", "38d875227d6917101714eb97ab3fbc619304100240c72a7ee8d22c68126f40d32a0e643d3def1cb771cb43f785ea80676091939b7270b024c7b4a2676c921631" },
                { "be", "0b7f67c69cc59b6cefb1f346affed1657cf0d96a5014b3765599cff818e2beac23568a7e2b3697ac6b56254d847ef4636c3afda760154728e0d47d787b4ca5b9" },
                { "bg", "45554b1b93dd258d921bb2f11e6503bfd3d16f8dd69498fb9f2c64f315bae12557f87f1e7b5b34e1e49509fe53c5b015fd80cbf0117ef000e03432d96759b9ef" },
                { "bn", "8ee529fed55f48ea9a932a541c4cfb6118453e986fcb05ce9f90b4ab8c7902892af959dd663f00fee893d12a7bc160348fdb363482e2d56fc24581317785c6f7" },
                { "br", "7c2d617cc26cba6e4e11a8c719b1d271123d348e691729b17c7c63eb8917759f35a3ec2ddb841269dc3a28b03944b101d1509c18a8554767f65e1f1c6769faa6" },
                { "bs", "841a0f21bb388c87bcad7eca724e32e6070cf74e5479f3939474923d991e9ea5f9c8f2a5d4b304ece165f02cbf78a19d1f55b48e6f29e26f698b7666e0d2dd59" },
                { "ca", "d8513fd6c02a949b7ddd6a70ae6cb2c3508a60031c0b3e6bb89242b928cb59a94d8c1a6db3979d204fa80a895b1c5480f7658f5a518eb31d4f2284b8c31fb6c9" },
                { "cak", "efddd10f88043814e8a7cf859aa539c74fdf7815f4412ab790948d121350eef6c0f35940a7d382e25340cdd559b978f7bfde176d879ac106fb83f4c79391fef7" },
                { "cs", "fb5582f23bd8b9664cc70cb3f6dbb669a8e4a4e408716722212f36bbadc0233daa76fe5b4e4a2fa7dd6e36156508123f985ac693a3e68df5f24506c21d35dffa" },
                { "cy", "b8748067c21d1040256fe807e43f36df5d78708ef03168a3f858ffdcfb87635928ab8db168e2c43c7fb714ee02e3f5c0ae70b075376a55dde790a80aa6e98b9a" },
                { "da", "6e19a5064b4edbed0ddc16399248b7b24d5ab3da9b9d235cf4ebaf9b2463762352609039aa4d5fb5ff4be6e15ae7b4700cb969632635ec79965c56511e663038" },
                { "de", "8092a9bc345df0ad1e23974f742679e234cb3ae984b228df095fc70192181c7f9681e0f43856d0d642f70fd2b05ba92428eb2938e0c71b68bddd7cd72deca493" },
                { "dsb", "1e67e1f3caff80d88abab2f657f740f0c781a1d4f7e269089198859f517b93a378dc1bfa670994ff093ebe95128858093708079d2e0490920539e605db314bc4" },
                { "el", "779668210807ec4555ee3e86d974714cb9a111f8c8b8bd5870dfeaccad3558c4ae52784e587a93d43088233578cbd6e45afe25496dfb8fa5c979adb8533eb2bb" },
                { "en-CA", "756af14596b890d41ee7b50b7ab20c553804e2eeefefd7f3a8f78465896a6ceca37ba73266ec828b3bc22453e2a97406d36bebaab4e996cc3ad1434ea6f77ffa" },
                { "en-GB", "541c560e5cf5fb8fb6a0130f149a4c1c9fb5b409a3700cab72b1075831c2f112a1a428a3ecfa3ac084f68e5d7e17f77c2d81b3f141fa55eb91d6294424212187" },
                { "en-US", "968cc4ca709e430c4d98b848e0e93a5b9e1b43e844848336282a96bc17883dfb2c2824de7b8663ffffe103bfa95787d28a6eeca69ea9b9ed82b63ae70ee77492" },
                { "eo", "8f23da8de3f484acfd5bf49fa109181de6bff9c6c283440232cccf86d0c69e166c6698928dee5d4dbf3243dd8a7a7bff5f9856202efcbee2c3c105a2a8af2b34" },
                { "es-AR", "843b39acece3ee6e10ad4981e96b1109021fc91a52cd834daf89d29bd5857925ee1999e2c6360f385b6cd17f285a6e66ac0ecb54ebf19f6deb94798331165e90" },
                { "es-CL", "02711742edea0898eb78710c5a4140e66d5c6a8f6c5801b1b5cb0e7eac8084f2f9588dea683bad08810b0c66f78409be11c4dfc99aebb16faa22f9422716cd91" },
                { "es-ES", "a063439e1887acac93d6d7720f55ba56613502acdac3a71cb5da75cd05f15808f9f8d20ad93a477bce9da7c3d3d4ebfc6a2cf0913228174aa8fac9c90cdb97f6" },
                { "es-MX", "eef9363ef51b61aea72d07b6d4142a3a7c0aed95f58bb9dbe883b5c355521422ef067343ee85d0742291d485e29c66a8305c264217835e14f95e587fb611a84d" },
                { "et", "5ccfaa4700b314525b27a44ccb56c250e4011aed6230c69b47b07a4c72de1b09e16a80d2ef0d3a03cef8c86f74764d4a7f7cf78bf5c566cb30c8cc9269ecbcbe" },
                { "eu", "554b31fe4d389b9f3aec4da6bb31333ac024e56f0da4aa53e853a69d469600ba74b7b71bec2ec478cd774d2586b8638100d90b10ad76a2232771186c14472e59" },
                { "fa", "693f9358ef8ea6d44ed57fcb4e0871b604d6f010ba2520eb7ea038e0492a9af922daee58f63810d8f5b6a5664b740183f58ab882801ed2d3897bcb0a9fb841fc" },
                { "ff", "453bb6c808bbbc78a624e3a66aa416253bc3f32bd9f46df4e8d7c3400ba634f9ded99587ce2a441a11efd95116f466f9991ef235c7114ff0ca38f12fc5e7fd46" },
                { "fi", "185c4ad9e9c261434a7260dcfbba50527a842b20a828f46347326c5be3c5fab55a32d6326bb8574f264ae1e93f7d6ed86d50f1935f66c4801d34a050643d5395" },
                { "fr", "572b641383a447dcf2f6511f63b89e0a970cae596336213510792cca63401faea4d93626c9356dee494cf05070e62ea3e46a8da50852f10cc1bf4d8da233bd46" },
                { "fy-NL", "7365c70371a7e61bf469709a9275bcb1fab8eb4cc8181755f44c6e8c56e150265a8115c82ed6a79940575d00e9b3b08334d14b7f7d392954528ac6e91a889598" },
                { "ga-IE", "f5fe3e35bec952f25a0db7fd3176aa0fa49c574d4a0509bc5e67a9a59a01b1dd00cd8f122b196d94e3f220aa948d4feb286f0264d805dd3fba80c6105996bc07" },
                { "gd", "a9618ded1eb1292a09774b03df195133922eebfb440bfec7e227ece0ad5b97d0b573662f08c75c7569e3511d07f0ae9c2367ea0851bdcadefc4dd5f22780ae30" },
                { "gl", "011493d8858ed6346139d52b54dadce3b166ce211e29c6bbd28b01d08a350a152051e1cd0db212947a8b364742d3d35fc233b20f64617a8858ebe36d79de2a79" },
                { "gn", "d3830426048bfc2a04d45b6eff8446c928387206d0c62b8ebce9c62cd5e79b32c9e07acb19d3afd7b16e1ce2b7fcc16574986c7bf6b97a1967e1661625504237" },
                { "gu-IN", "2e1ba62f5b06d7a7be28348d411d2da601e198d482a30699e6ba9561eb351a5d302d5ee9d2b8ae59927924c86cb76b6246f039b7505ebb344c567c469a826c04" },
                { "he", "b2ae4752c77fd8313354783840f390bb195a6ffacb78f7c5b85f7b8c05609ee39ba514e32a8199aee7e6812aed5b8ebc93e07945577410ad18bb7140b4311301" },
                { "hi-IN", "7044ebbea18c095b1b1bc07c63cf77cc1418a5e7845edd8ab95c48da3c089747d117d586785901a975aa886d547f2f854f45cef33d78bf9a6bde1d4c288042dc" },
                { "hr", "01597c716c9774d1b511211353186bdeb756af99713d9776843fa5894af558146db91ec2f1cabc42b8aed5c1c87391025afe333216479797244435a218e288eb" },
                { "hsb", "c6760eae32d0dd6dc8dbf15561990169f9c8db071ed03fbffe1c42ac0919f725943b1b3135d43096d443510f3f4b5ca950c89a38f4504d23a883133fb3f33236" },
                { "hu", "8ef28c5b623cc2e188ed34da3a95dc56f5253b008ffafec93aa567bf59592367e4c71cb8a31072a6abe2e5aefd1fd772138fde1bf537f91e643470986bdc045d" },
                { "hy-AM", "57c19ace67f3239d136ae79a8dcaca641e7b8bec2823d238f4aafee7fa13f5bd2fda03d548a5147a1e08d21c95fe6f84a7cec0bf6dc9f99db353ac3799922e95" },
                { "ia", "b0893cc81f5d1354156b1eeea70228572170404892159e3f2a7b08027cac68946275a705b4b7329d65904d0abb752124fc5b8405289ec5adad0459cdba13184e" },
                { "id", "42cf44018f5c770b228bc45d4391f85d64e4620571c75adf9c80662e3496f09c0dd96a35648ea379d37419703b3f93c0b58b273aec0f344421c7089f9512dd26" },
                { "is", "8c5c8e1285e9a185223d6783791fca728628c3e29f2dece2ffe0d35b6d9a9c805e908fb72d7e6b6bff2691ba5e3e87aec660f2034763ffcc39f7b1b7dde67363" },
                { "it", "faa4606044852dd00901c40c56eb5a3031d9f8567a11489b404e436048a64dc4cc2f1f913546a2f5591b0a369becaef881982361f5bd570a8326e6287275dd21" },
                { "ja", "1482052005ce2b4b763184e64b71b0300629e398acb9f5fb6fceca770bef3888090872da23d6115b3f3e2b229580a9a277c372b09d1ef00716e407f42965a12f" },
                { "ka", "6d278382e6437ddf5112478096264f007d93b22e52820288f65141e21dc7b1c5f92b82d1205f34e07ec212b3c0dffba0c1573c306fdc1e227cde495ffd0d22a6" },
                { "kab", "2cfa59a51f7f27648f1028ee1127ba1b6b77710306305a12037788478f9153c7c80f4183eaab579d4f4f0ea5ce39108a296a902fd6b3c39d98264ea96adaa3b1" },
                { "kk", "b3d9f7f2d22a569f61817cc1e13122f02e187774039880b79c9479b06338f55c57a1d0f6144e98fdb2ce92b453262777bd15146e44cdf4713e29a4db1942f03e" },
                { "km", "440336be4143553ddf004bacb7694036799ad3a11103d0bd89a3f23a9a0753924791edb1fde526cc1a38e6778613d162697019bbde6261ce13f8ccdf8221b8d8" },
                { "kn", "ec0efd687c398fdf32d155a03ef9c4d8fb2bd1d9dbcb16180048a3af31fb38eede0d2eae32a37583ef3a45a8799375d5c13132601eed48ed065ec9512194eb60" },
                { "ko", "1b2d14df7088ee2ac704714090e3b8edff49dc232defcb819e4b8acae245a9472b312a792c37ab6684254dc84f0d5e4fc82184fa4ee46b4a52da6eca65d005ba" },
                { "lij", "6e27e539760c7fa4656d90fe5213dc196f1584e8ac719a41d7908538b100f6bca3264ac070bedeb9bc4f282afafb069ff880d89da90130ac25af265f566dd0c8" },
                { "lt", "5b2de437f3b8462844611ff2bedf0a93a8147c369df6593345405534d755a5f14c80031dddbddb0482bbad7306a43dbd1a8c43645385cbd3e44346abb46c19c5" },
                { "lv", "90989703f9e61e0bc77e20ab8f3b1aa936a63ff8b67cab1efeba67c6db275b9e1c4499259d4b0c119fc4150a0f7dde0ab53ec5b3f4a62cc986f428c81796d0dd" },
                { "mk", "4e9e37d989637256bddb814ad76a7b90057dcaa18f6726b2a2678aea3dcc138d17f223af94e0e30b6d691832c206d844381b2454de5aee5e8fe556b7349000fa" },
                { "mr", "f0ba6f3e0d49158549932d5dce515644267802b387c79ea769c8b3358ab2f8975e46d460a2dc53de563a9430ae2a4f97ee60c5740cd58a65d09f7b14dbb12023" },
                { "ms", "a9b92c435275c32ffff5f7a0a77631f8909ac9423786546da50f6015e108f61ff43e629bb195c25655a6ae00fb0864a539e1afc7ec111594b0ca2eed4ed2b9ec" },
                { "my", "77fb4e19ed169c00560eb6e9eaf0a9356b8a5ae74b249974cbcf46762c622bcbb77ea3961d3aed41ae721fb31429bc8ee7e75b34681c4f23962f4678091d82d5" },
                { "nb-NO", "b263255c6b699e9d803c162c00ebd5241f2d1efaaa602bc59dc825f17532b2e02fc5a33c60a8192962c60c32832093a855621379603f41279cd7625bd0b93ea9" },
                { "ne-NP", "6c5654dda2a040936c0ae24ae77b4fbde2c43f34256acc6c6d037f2052426384fe3cf395e21b9d673d5cc2490719e8910f563bd08efecbbc877379bb13a74e7a" },
                { "nl", "fcc82cd51d9ff649362646802c04d97a7de15fdd3e06110e70aa86a44fbd2a0ccc13adea89e3563deaa461f478da3f8131c735c0505a100efb09fc07f9aa15be" },
                { "nn-NO", "d1f63514f7c16d06a13485e5ebb7ae4ed7cdec60239e347217a31a9bbc46c09341c66da737a3226a64646e6664d5f7fbcfdbb9412be6ae58280f0f721b2c3bf5" },
                { "oc", "9c63e12832577456eb91083669eaa0aae8d3f34d734e33040ca9fe647c48cea535a490f881a5e0b90867fcc9bc4ed6060c615926016421cbc197e9ebaed5b0e5" },
                { "pa-IN", "b699f60358be0384848ab4f5c874f9cbd7f6ddd96b9dafabd7f65e3df8d2fcf65374a6a2de571eb663a9410d1ed7865e205c038b73342ba038ac1ec516dcf691" },
                { "pl", "f88862f6d48fc974f78d79a5bc2ae45cc419b3f5f5ba2440cef3d474061bfbda1ea6b6e8a31c6e631ceefb0216be09116a297bfe592eefcfc919ec366d4dcd3a" },
                { "pt-BR", "06547552acbb004343cfa4944404999fd870a61738784b867bdfc785ad26445bde40d7df9c08e304077afa8f9fac85f17feb195ad52ac1dd7286db80aa790884" },
                { "pt-PT", "20292ef4d07a8707a003fdfa9769f99d43cb2bfd179bc6dc59e653e8e955feeff356d90c4b59303864e2e1bd1b05b395c089db4080108f39b26f6448597af3fb" },
                { "rm", "b13de83a0626e43149826220adfd937df1b421f32cc460071d6882ddbcf86bf09f12060032f05a04ef46be125ea3b2766cedddab05b88e296f34f52639ad302e" },
                { "ro", "ebc1fbc380b9dda7135ca939add2ea31661856441f574583fd2185129ff08d1e939bab57e6ca761446c6a4080e72da577195f73feb173af58a2817313f06eaee" },
                { "ru", "39f280297350d314c28f257699637e02d1ff77dfff347483a749e817771ba1c2f272f401b6053e38f2f5bb87a50ae7463959822e433a06a1a0bb8d75dfb3299e" },
                { "sco", "1296add0ce29e9d9a3772ee79b5dea413b24c9ee835a6fc5d4f3ac41505e0da7ee14a8fa0d1cccda35adc0e496243c533b014d033d91d0591aa07e69c39f4989" },
                { "si", "6459b795b7a173c32f9bab5b16cd15128dfa2393c02270027a7f6b6a558c46df2b2d14ff5b2ebd86a3704ff97aa03e2e7ea05ccbeb8ad9ebd5ea2c6a7fee05fb" },
                { "sk", "3028fd9fc49a103bace12019cc2fce4985a1aed317febc25b4a212ddf9a50d93b2a6d11637531db6a34b737d6da4817945d328cb3d188afd3fac416327882599" },
                { "sl", "f1ccf55e2166604d280a0a0d3bced2ff13c70c17906eea5ad7a72be35a962d482c101239f21502908e25a795628b5b9550a6c37e9c572b77b39f41c2e61569bb" },
                { "son", "3bffdb3d110408847c2255c4f5939f07d967c54b1e6c992bce170515ceacf7dd22c8ac90fec6340f3f911a5439cbf950c21f8b87f4db54ad7fb653487e2389e6" },
                { "sq", "9055a4ff73998f5b5b7d0faa175dc3511bdd3fe300104e6059abfce9bea21968a8a70e93c02629a9516490d55da27dc91c6ba1b5227d3ff32cef609e9f11a0ed" },
                { "sr", "b3e85c89d8e7d2ebcfeb46426449cb0f63d55ccbaefc9293ba76f1e788737be9596737e8efda7199a69ee319a67e13d254e1c062da832e0cc82f330bdf5821f8" },
                { "sv-SE", "468a22439ada95798b95ee6fb54529494a2ab7e8f05750600767d905e04b42af93d361d2ff4e36b2de08b8a869e9f2bc8ce4ef3ac78b946a9cb35ab879889610" },
                { "szl", "d830a6be383a8bea6b517b3bbe910c601055f255250b9c9b8f2fcad0b3ea24e771469d243de50bdf948337f8d6d5f3f2077264cf1161adabfddc426ef6cc447f" },
                { "ta", "911b630ceba7934c092de021a8d0aa0f5f7d99a78ad8098821c6403ce944183e174982a818b691794e024f8f2a421ef33e740e290f990b9f6a1a03300a74d3d5" },
                { "te", "2003c049a728ae68d9249218cfc80ef0eb11eeaeffb0fa385092e69e4d8a896a61daf4e3fd66069fcd68155ab3add22d3e0aa207845afba9cbb6b6fda7bc084e" },
                { "th", "3ed584029b094fac9f521593e4e83829cae99b7898c09d1c41d3021a93573bf0160dbb121c3f5a7715c98bec12e0903287f29481ccf80836ee2bc44bbbfaf151" },
                { "tl", "9d2d1097d54483ab2705a053dc75559a3bd0fd65be9ed68fcf78a43e19c37dda840c065ccf047a25d3d812ad5c50927817230d7338506642b28ca99d6aaa858a" },
                { "tr", "023194960de322eebe543388f4f95c418dd89dcc2c9f815c0a98e6f4edfb031635386f47192ec8b78df79b2bb4ce92f69e5a8bc84732b68ee6221a4cd474d7da" },
                { "trs", "3cf64061b295843e03f2b62cb1613f8e0701c30d688fdf257997ae22c188539d62a897fc961737649fc577c7218238e04f55a2de7c2350fb72e207b489b5e64f" },
                { "uk", "5622878f7eea29995419bfddd24c7eef6e32bb20085b4f6bf262f3d1b746356a6ed6eef45da1f17ae466b8c64106e953c85d79f15cbf483ec979e2172888d238" },
                { "ur", "15f709b8a55b64f9ea7e59657ffa6eb22b30abce24d7137c2ec3bab90e64eccfae664aab9c51fc110693a9eeeb84bfa23330014d56cf5d14c3866301d25cfde0" },
                { "uz", "f226cfd714edf1ec9606a6ea87e768333c9844f338e32cfda58043f51b0fefcb99a3d1c5fa6b0995bd8be11014b27dba887d282f3f8fea0281b8c352f2f0fc44" },
                { "vi", "ff618f729ae9c94ffc5cd0e8a7c4f267628865146546fd1ed229e2a4b316acec92d7b8e43090d23c02437008cfe177a3443f453364b47d127885a45b8eb87271" },
                { "xh", "24e8ee348d72e4341bb8922afb5b4872b787ca05dbe57afd42624ee7d915da666429bc85b34d3c303ec2a2a9f81310789b8bb12b120421c2614f02e4e6a2220e" },
                { "zh-CN", "9c51cc932c00b405347e1d4797e24f20c2d3eff619ce14655b4cc2d4504c91007fdff45d91461477e002bd2e0aedd3f6ad7ca7719053f242b6b8b5085b34ce95" },
                { "zh-TW", "73a650441ee6fc7f5cc194a3999d0c435e33b4615d6d527f0145880634ecc839cda93292fbbf18a3ee96301a7a35aa8651632e947007ba9409aa07362cdddd0f" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/108.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "dc00d5b09f5bafe7c0a3ace78537061a43ff25dca6e14fd9b74ddd0e554374193cb040c97a09ab1bfd5b7b294a3106b6c38824d2e158b77400dc78e0af7b7d7d" },
                { "af", "15d01500b21448e0d187e9e8992f2510b2bf3dd01e78ad0d77a68c4ee62754151a547e7d935793c11d3bf6d9014dcf4d38b4d6f5abe861827c35c97eea71be3e" },
                { "an", "7be6ea93adc0646c3c830389fb3773a12822b2712ba7ab5dffd49b26861492aa72bd2e3545b1cd1abeed9906cb348e897f662f7a910ef644ca6184e4eabf0344" },
                { "ar", "bbbe6a738ef16debfb7006db1c9509e842e6cfff02afca7d6d4ae2e6f98bf4d2464e5cebc57c92a1b0f7f6653e1d3b92c6429d5b5ac9d115f4acc3a1380aa539" },
                { "ast", "44404cba870b9da5b01e5924d117ce8bc613af8ebd901b9c6574cc90d4f077eb50c444b82ef41859c833483ab6aebe58765e550f85394c4132d8ffc1445eabd6" },
                { "az", "a5827b049b462ffbea2b38f727716d516eb913b7909be6528b680cc09f32ac1f4763c8151cca18bf4ccd75ebfb90f96ba46319ae6dda18a6659197af4f3399e0" },
                { "be", "b022cb5d4acb525d48d2db6084cace56eef56f66c44e44fc13e81e934cd52d030693df54fa3569668dc093d1a3838f3363db2330b40828207966b912c1815a00" },
                { "bg", "3f2599af42e6dda716067c22355a0d5c1c2b814f6cf5a09272c505fbb68bdf9ccb6a2a7d60bb93881b0430848fba6f6bdb011135b38128291e3e9712eaed4d1e" },
                { "bn", "450a74429df5045faaf43ae7131bb88c469c845c4efe790f3038e87d9889e6da2d12b8a5a378c586f203927ae9a7d20bcf76f12367d66c56933f4e39426a15bf" },
                { "br", "040cb0e5efc42ba460645b783106b0ebd3611d25fb58d98445be71f5c86ceb6f4bdebe865d81b5cd4435ed97f04a081a8f1c2630f627e5698c1791313a5ebe6e" },
                { "bs", "663879e49ddf87a7b8f8bf58ac22ee19e5642e505d835b81118a010588b0088abcc168ec30af5b06236f958f5960948254fc5068a0d0a512024c47df4ce644ba" },
                { "ca", "4986afabe9c3ff9a73e39a66a4a0d40c1536d3f46a2e66daa8ce0e5d3b6be87132fa0aabe7ec4a15ac54ede8a1282af46cb49cf8d6a9dc8117230a5243d797cc" },
                { "cak", "5e28147fffc4122447fcd9ac79be1ed21be19eec15de19136068df1c5b933d71887e189b81e5ac30e9d8184f9c049a1928afb36372dafe426564fc5842728c35" },
                { "cs", "e58e10320d294d24c8052787ffca7969eef9998134b6d05bffef3b82f01ed1fa9bcd03f6b9666abebeb84123fd3710e8474a2857eb20a0617ced2790d1950aa4" },
                { "cy", "9bca486a5c65bb4f39a845ca1982e04bc00c230b55ad4957288240e98004fd429686ea667e38d6f769990ca88bf4b96075c9b579330b45a58736dbb777cec25c" },
                { "da", "d32788053cd62f89f2c1b6dd6888eaabff5ee3f6febfbc7177c7880298f0df453a11f799b76eeef05e084d87e11eb61f14b3c8f0a88f53a3c5e9d3fd77abe884" },
                { "de", "68b45cf096cdfe1822127c52142fd3b5308760fbadb7a81e451dcbba221fd3443f3785754720a8a05d304a3130338ba5644d6f5bc7fe14d06a841b22abcd4749" },
                { "dsb", "b53559f06a6149b0fffe17b78da2d882454c87ee0e89a42f396a9489a0b443ae17cca9b6d3e325df253097b070f40b02144bea0481ef45311f0e8463981f702d" },
                { "el", "6977eaa916277811792c9fc3c821e3977cd7c8557ddeb342b92d7fa382661b1d57aa3c70fae5a26fbac35f0ac76c05cb80056e25b24b71d3fe79731765ec2e74" },
                { "en-CA", "62d3ed6a971eaa7300f0174c3afb14538a190c9a29d3cf8a8c0f49d300566a18b6c6b05ee35def195bbff276302c0a40c04d2fdbb3e34495f7e715408d4effd0" },
                { "en-GB", "36ef45f84754b1141bdfc3473f250881fd54f7d67689083551036cfadebaaf63f4b05e28a4e7a073cc31cbe44d84892c55bb5e4e5ae96d2009bda33b69585f4f" },
                { "en-US", "a1094e25dcf8db49359a3092754b36acc332cfc397017854f12e6c5653a35d39230642b99aa3d52569a8e3dd16c5d09eaf03ec249d76a74e327b43c03ec10633" },
                { "eo", "e30d3be292af5905a738310b04999577a061ab2d291b44edce799bd6207c7798bca527b7d6a8ff3714b786a120df9d1acb5c29dc034b13a144d1ecf4c7579625" },
                { "es-AR", "e9bf89fd74b4c08286b6f12c3e33b258df75e05dc6b4cc4828f6b86965c7f149d6cf048855c2da906c54ce5252cc95d2cbe29c2475e357ff1d6ed46180b4f976" },
                { "es-CL", "112d95bff6927c51815ca0b27b5f26b7db635b9e33e179823a3f4d678e016e7d7fef3ac29485ea2b79d0576dfe1419f6acb37fea065b07d77e1b16054dfadb5b" },
                { "es-ES", "e79e88300cf7b4b5f5ae0a94148b7e1356ff9941b21c507cb866a5f2077384db0222875606d90b374a318ab0d4869ae64139f70e57396cf5e7455620b87619ae" },
                { "es-MX", "e4ecb59734fec9b7110d6d50bf2b633520913d58fdc33b9f8990ed8493591702dc36a5359576eeb4ebb66cca18c7b0675a07bd917464c8ca207c5341a9614bd0" },
                { "et", "5d9c9e43f5f34c8a5f31483b05c80f1144b5e9a71d346804569f6e1b937634e8d897279b5bcadc9e0015dc7774cb97af9ce4c3bf9fd4679b3c3c17741713000e" },
                { "eu", "0d8280c7633585f883c1ed044667d902723593241a71b1c309ae26fb29a9a2cf727526ed4639347c846b512977a2520fbd5e2524a3a977a64044986dd3cf9e9f" },
                { "fa", "47469b739e1d043c957072f37747d55d644b4a321827d25a63dd72c1e3ba38f360293c4474d626dd2d2f6a3b8ac810f38f3df14c42de38b2b5a508828e3f9909" },
                { "ff", "3e4e0a270cabbb26151e7d3188e308f8d9d821ec69fcdf24256d063f66be9235e35b0ff860196a6139338d1386243e13b24e6c7356780ed7e7e47d1ab79e364b" },
                { "fi", "cabc1e55efc72f36a659b5257147f159e553727579893eecf294c456bfa5b6d235e33a2b823a8609b62bc74ddf08ce864a305969787ecc3726a1b8d4ad670cfd" },
                { "fr", "305c3e92e1df4db5dd5cff904b74d2faef4cffabbb7990584035c9bcd553eed461857bad2d79f7d75398e38fa7602e003bc84ba5665728aa4488b3e577f82bb5" },
                { "fy-NL", "edcd14c9d620ffb126634da79b74f8680426957bd8b90fcbe91fc49a70ba4015088d02b436918efa00785fb6a30990559cc3b0ce2c61b8597ee63ba95f5c941c" },
                { "ga-IE", "323777c2f3755fdbff6097f11a1f036393533787716ac42dc3842cb4ea3c4d9faba42721ef2cfdb6c6104cd51a34de490b79b2e335e72407394f7a9fa4035add" },
                { "gd", "47ccaef4427ce31d63b20f3ed8837910bd3fde16f64423f758e90af946fded887b1f9f5e317cd52195deecd0a6fd1c6eaad242e542b6c2e0aeacc65fd62457f5" },
                { "gl", "f4d784af50134f098d49a88b40a3aaa824ecc2c22e107c4095b380e55c5e14fcbdced550560955727ac98a44d4cc35f81689ee93d655bf45b24f5e28409203d1" },
                { "gn", "c63214a41283f1bffaf8f75bd3eef03a773070663087dc646315c589786fa2d4d35db9eab4819e2126872bac8f19953f4699c9899a898297f73bfea99ecbbf7f" },
                { "gu-IN", "58e47c49b3f7995086ca01d0d5eb277f7bf91f778927ed1c3e31453cf314947095905f3ce75ede7a424f3995266002f50398195c05551f1e1ff847dc99070259" },
                { "he", "46e102fceb430ed59ca48bc52c5e06e56adf2e3a801f586bcb775bd8f3610dbf3badb8663094ac23af1cb0241d3565820f7a8c6c00ba7e4ccc41707e83467396" },
                { "hi-IN", "4f7569a9815459be0571d304daf2c9d92805b751fa604a3162df6e0e3cbd15f971eec71af3e31f340528c925b5fbe64695036c0c416cec14bbf7bbd09b046d7e" },
                { "hr", "bdd2945fbba8d7a13f7b48895ee1041fa71dc97b2e5983d235061abd83d56edd4047780639f000a5914b660bbbd464c73827c6065de2c02311404cee869d4a0c" },
                { "hsb", "4074ca99ff8e9bb41b41878a9d0a95f69d79aa7c26b8abdf136e059837c9631c7275e2371d9a6b6e95327ccd19e71593a2f8d7ae2305f6599d271ee88d448c69" },
                { "hu", "5d9d46187734d8ad2828a563d51b8b5affeb6e2b26295a43d07eb1925b7f81c53fc055ca9867d62b8ce3110b796c450229317badfe45460345f8eb71b2b3221a" },
                { "hy-AM", "ccf7dbc76d71d86ff161c7e498410009dab396ee81e9de0bbdb65e7f0641a252f4154d0e54ebe144d628fcdfded6e7f37d50b55e19c2a6dd5484c5618e9fa458" },
                { "ia", "c091849ee3faf59202988d9acfb9a41db45129d7c01beb6e57a3905f8ee292fcf9602abfd7afef7b7276644e8c4e4ddc97c266385b4c502f14cfdefffc887a7e" },
                { "id", "4f4dc06afd47e735549a1ee8552d9f0689aaf43cd9b360a4977414afc3d9e8da4badd641afd9787e80364a62cd09c637cd6afbe9b74f553caa7cbb32c484b84b" },
                { "is", "cd0bb2223e6ea8c75691523606ea6450cebee1e8ce595509162ae9b1153f18934772763d519074fb2d392ccaa7c90e5254aa1e64109dd6b8811537be3016bd4f" },
                { "it", "b92fb71b3c870435e7c6b81c19abeb8ef6d51672e19955ee644b09d19db47750ec67306e933726c074143114f1fd700fde57061902ea07417a0574fd43caa43c" },
                { "ja", "5c32c925c7995f14e465b10429470cfad58311dd783edc84eb3ffd2ac97912377420d436acb141ddb0f324e00dfc0f2f44922cda8d65f13801b9d5eaa46d4f09" },
                { "ka", "504bd7d8c40ee2e3c22d74592461e2a167fe45cf71f24d17e21689550989a59a629f58b4a6ee102f22da5a22754934adf077fd44e05040155c1c7656366d9345" },
                { "kab", "79146a8b0c3a012ccd0fcdd8d6b57d7e29f0fda905134a03424c7a20b88cd70032ecdad4cd7f34c5c15872e6ed0a520a868cf1e020ba0bd7b8ab0cd0eb02cf79" },
                { "kk", "550672f31b71557a834dc6dccd569e45c9de49e5ac6150efe26a767f7059d7c42f18bd76f2059df59e957713afa8f19f1d2e4616da2bb868853596a53f40ffa9" },
                { "km", "4c8784039174fc3f6d0cd9f9db2d22523608592d73ea8d69baabbc8e4af63b557381bae4384f7a2f51d3ba3a1a1ae343915feac561ab7d992a7e9c27684b8d79" },
                { "kn", "ae36619ec6c6841389e7cae4d8c05a104bb81c27625e35f2aefb2e537536343bc9d3239348b7621a3b6dfad7b8c788054dd36837dc6a68bb67ddfdd9a6cf7ada" },
                { "ko", "94005c4ed60d834d31aca6470d0e023e85d0b462feb10b821b1a3194fab6f9026564b04afee18d03862b9e19809135ac690bff2c7bb19d7e1948472b28155e4d" },
                { "lij", "40abcb99d8bcbab3aba3a2908c10380e8423dac0ca604d146126b66e1eefab025b463937f2bce6b0dce7f7b446f9118057bc2775ce510a86f066a1309997bc04" },
                { "lt", "b4fe3b50a4d4334226d759229c0027aa56f95e15e9490d16d8c19755d2622ac1439127387293b18c0cc80d0914f5cbb76dc9482ab362235c83d7e2b58fa1ec1c" },
                { "lv", "9059be320a7b63378dd436dcd90509a4d47a305eb219890c7570a02aad1abff873ab934c737f465f6232d16006e401c3ecbc200dd3169c2970364dc270d78340" },
                { "mk", "3535e2f1dd42cf7d17a0c48a768603d67f35a554c66b6e78f4e03ee3d1f0cd93cf6dbe4801744015535b26caec1c7792fbe11a559871076865259808d5c773fc" },
                { "mr", "46bebd704e53fa40a99b607edc30d175b271e5a0eed6929a4320a853d5e647e7e9f1356463bf4fc74e98b7cdd6944be6f5c79323715687fcd9ed2c62e4fdea2e" },
                { "ms", "f9376d08cd48caab5f1633fcc6f1c7520b1c63031de5158eb052253fc85020d3f48d4cb4c66f3b43312313df63fe0585708cdbb144f069cb32cd39ba66bc7b86" },
                { "my", "87fbb0be9d4bdf2c2554daee4ae3ad54c13014d647de538a0e530c7d74fba91452804e1b63c46249cc3408be17a32d7e81ee6aa14ce6ced31a8551a0e8679742" },
                { "nb-NO", "be59fb4c616d21d5d5f3ccb364b571753387191e4cb1ad5fe81caa7af5ac5c687a3d18002043f3df1865f62b87582fcb18a15e85354aac6a3680dc8e92a818a5" },
                { "ne-NP", "3fbc99337c7906c7db92b149fee100d98aeca4a663ca69d3be83426eed05203bd2418ca7c882c0b52cfcfe2ce10888d2d14f5176690c10102b2aa3194f3b88ce" },
                { "nl", "158b1770fa3e945022fe12638aff000b14e0962066975f93c36ec7f4ea86b7097690b99d53b5fbf107c15508362d8bdb98f4445dc2af041b9e2873f974542966" },
                { "nn-NO", "3d04e03eee62a4438ebfc3ca140e3316d1382a25a14e8c74a805dbc4158bce94dc59d14cfb9e513c9fae95684d1600e2799565173b5037e26c61ba48539761a7" },
                { "oc", "396a28bd0902a1cbf1b63959f0d2c4d74a549e29d2e450209d3b962c4a641210362b27de026f0b3c5d792b22d8156008fe357fb4a5a8b1767e31806c90f75994" },
                { "pa-IN", "18b2ab527f5f906de4dda6aaa27baffdf122bb91f4bee7b165f9b30dd20176330727ca1a12a4618642e9dc76e1e6064bea4e39803968c50d7c5f1d4b574da51a" },
                { "pl", "4dd35703149de0314c40425aeb6b2087f9fad7b390c2ee56c5931a2da10b0e58738d93bdf5e53c543a5d934faecc829a2b1843a8c8fdff56754f77894a28c5f1" },
                { "pt-BR", "e80aaa81cf8d2900923141a0aaa7bf2e198378885f0545c8b13d42378ab82add8431695c7a79715b24942be148eba99897c191a6f0b1dc3592ca9264b3354565" },
                { "pt-PT", "633b54c9853b4b126ed50d6433ed70b96c6dbc9c51a014e6b550a73f912760f423cd5c6a15df144f306ff5c54f1b0880e5ccc78934ec79166520b62d3a73cee2" },
                { "rm", "67851d6c9ee44bcc595a9e54ac5c072749bd13029660c75dde16e3f1c6e00a1017a3cdfe93a323de9348d570349fc089cf6c6d2fb41cb6a0716615ed39059db8" },
                { "ro", "789e5a1ba1172e6eff437950110fd45377f20dd6613a0afa376b119b08335b9ee2ee40b5bd8974b5b8d54cf3eeaf6fc51a1c5332528c152565a0a9ea8175c4c3" },
                { "ru", "b7c0e531e5ca419cb3aaaa701f9ecf7131ab4e31d3543bdacdb331b45a28067abe169025caa87f4060c9b78940f2b07c6788d98e841eb9e1b3206eb900619935" },
                { "sco", "0826f977504ce3c52c951f77ebcb3b943c3742b27c41561eae05802b8dd8b1e1a3806495e3cd93e680261949aa4c3d7cc03f65308574277df60579668c2a4a4a" },
                { "si", "b6e12279137926d8e35a85aeeb9faad4c00a9321c0e77fe1276ed08ba9ab647c8aacd1959ff55665c388fce2e4b997515049846468db61965f1fc2376bddcb02" },
                { "sk", "17172d67e135503d37676afb42b535ff35cff5ac519386a4fe376d117965726862a2b2472e65a807ca571cba3f7cf14418c846ed9c2b318cf8d5d1c75772a1fe" },
                { "sl", "d1fdf4386386203e673aefa7e531bf25dc64283cb8ef76d4fec99d5c708ba2f9ef61d8ffa47fc02d9bae5cbeaa43dc2849a55578cb738da92c312d8433eabbc4" },
                { "son", "0c27ed6a8b1c408378c8072b343febab5bdd056b45d6d019074a0a883ade362a6418839e2127ef01f13b83c30a4cd5c51e6befb28eda20989e7b0ed4ecadd7c0" },
                { "sq", "5fcb2e49ac9aed93eecf3e926c747dbfa40c09b8eeaa27e9c8b74a946c1142787ef442641b085d5858a91f80eba4bcf250db69dc5f52982964437d6eaac5e4fe" },
                { "sr", "ede878d07dffcd89755a7dfbd7a51eee05ed2011166c993534f94365fa6d98f1de3e86e7f3c607026ebd21d6253e1b8be3ee0cc9aa38b2e026428dd64bff0537" },
                { "sv-SE", "b70f7f9a39ebd0681dce07e399d2ff000f7b8bd65f8a80bbb853ea52610c8e964b95ac658b78394e29f4e93f4f8b1d662537456e7d50d79c6331e07fd0f909bc" },
                { "szl", "9861b9b8d58c737ab111b27538d9e223ac1bf57da1c64b398da843be233ab8d6cfe37a5ea02716ef0bd48e6cfce281c2560bf995904881fc177536e87f998454" },
                { "ta", "786e643b155987a97fcd621ee1d4c334d1b23fde0a7173d33b2a08dc4fc43250ffaae5eb6e2be3d7a7107d36c0721e50f81d051b35d1f72fa1c1a2cff4012360" },
                { "te", "7e9ecca88cedd09122cf49f0d89cf5608a460c6d03cc8dc15e7c518ccdd593126f179e2c552b27630eec0c5ce1b0ca0b69f4534ef460b7887f73db33636ca1ba" },
                { "th", "26984c382025c6d91a433f60f66f03fbc86c10b51e1b9d8c62c8e03816284847d81db3ad4c7e00256d44357dcceb65b1a17ca172b82a4583d31af6321790dbb7" },
                { "tl", "6bc5d033267518386a44253977dc79c8148f2dbeadeb3b54512bdefa0afcd4af27cab398d750b8ea0237abff5e686cb2b93b4c18af2f66204563e68bcf5bd802" },
                { "tr", "3f753f17d7d20b2ae60c35888be66d5632f75fd4d0bd0564b4d032288f9cf0c8975a603aad1ba9f1a267abf67bb6fe8a84b7234e996837309760748aeb32167b" },
                { "trs", "bd5526676daf432460b0e7804d82e7336ed69d77c57c2e3979311201da9a731057f33d1b5a87af2dcc42f7d9e2b6cfaf9474b30a558dbaa32d3acc539ddf2bd3" },
                { "uk", "b6d24a6785866866d97c1376da1dc2b1b08df1ecb1b9a8c0dc89c6e63168c9b874a15c983e6462d7529ad4d5c27e3d21735b14fdfaa48d4f0c2ae0dabb91aae2" },
                { "ur", "05c5707fb25dc6bf4c979a2a11ce788b67bb07979329fbf6447c04bf1204b42465e291cd1bd9cbb2260fd53d80039674db0eeb496cf86d74297e7cf568ca31b5" },
                { "uz", "5eedae9ea16992824e902c94dbc332b5d48cf4d4aa5b326810e4789125445d2b649ee18f2f101b9eee091457daef9072fbe589f464f2644e4c3173d7edba3279" },
                { "vi", "daa8b2ac757212b12f63a2285b720b1ba36200296e186d5b5e54cca764f06980a9d362c9b91ddebcb9672063a266bcc76bf1411504e9ce2602325ab81d9a5438" },
                { "xh", "24f2bd3f866a8d7486288cc62b6568ef3752e5cd9441cd561c2858ea5b25b49f00e95a39a7d30789ee8235ecc8d0b777e5a5995b698a247b41a7037660a1e107" },
                { "zh-CN", "54bcfe37f231464ce6153f5fb9518b7c43ef9f763f9e8be529cb69af661fe88f033515864255d5116926f0c6af108fe0f8d1936379b29ba77a52b97cf552903b" },
                { "zh-TW", "8292ac7e768cc2222533806bd2a3b5b2e05df7cf4c49b10bc5946e3c81b3d9a70eeaae27d9130f2058f39b7dfeda6a0e1ec0de6ec8dc69c2992d92f6678b88b7" }
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
