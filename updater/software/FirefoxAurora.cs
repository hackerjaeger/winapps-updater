﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        private const string currentVersion = "135.0b3";


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
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "65f23a9973db4cc82012212a7c819c3aa0356870b5d3872cda90f539464f94de0aa1ca9b9f45d709cb4b34eed2bab30bd012ae200d92bc418bea57e7e953ad95" },
                { "af", "dc8407fad17555ff0fd8774b77ee1725aa8f3a516ab35036881c19a84a37739c988dd4391022b4e509d69524e1fa4abafea982a8a41f58b7227aada395e08361" },
                { "an", "25a360733c44fb88e36eb4ca765a38aee7e01378c21bf15724a77a8d738413b9cc69fb6699d96973ac49e850972d25d1c982fe0506632254b98ae0eaf815a20f" },
                { "ar", "ab8c461936862734ca96fc0f7b71af42f8097700a9658c171e6982c2e61ba9d5795045fd33845ea1bfa2972b9be9311f07c2ef8919f5cb0a97a800280526d3ad" },
                { "ast", "74cc46ea7d05a5508e6b6c769a1812b72ddd0462927b93d64c9ef412a00802b44bbc4979c4c5b85a27fc2f755bca03bc608178e631982c8fd32e3320656a2628" },
                { "az", "12bc544e96aa038e510bee4dc733be88aa71a1d7c3485519675023973da20ef81e1ff1b1dd153f106c03a0caaf3c178fb3f85828f45ab9971888c76e95d62a01" },
                { "be", "925f752ac034513c9b63a6038cbe293d33a0056c5e4a5f38c36a5b8fd58705b89ec242dfe7ecf1579fa29d3c7feef80fa1800fb1c8e05f928f51db4e401b2342" },
                { "bg", "c6ac0c4a8240b91883d1b3a6d8a794da834b39d50dfb5278d9d4307901f3e2fe48198ce42e96add498e773726ebacc42048f1d6324c252916f25b392d0b39b80" },
                { "bn", "d5b41b5648b4cfca251a956b80fcc980169ab71b91eead20e2f713121f8712b19f5735a559ea356c6b13520897ac2978f6ecd0e7add31f71fd73d69c904730f2" },
                { "br", "304a274003c8e3e15db48ee14def7782dc958ed5a53a246c2e7b3de0410b1bd99a5ff52f4a64f9a94e31abde17418808ae110501f2be726b5d96d1d29d94e22c" },
                { "bs", "9ad1baab32ece1bc88bf95a21e13b06213b144729e67d61ba179effee3ea90b6dda415c6bc10ac6673900397474e91fdbd804d56b89a39ea5b3e00eebfc8bda6" },
                { "ca", "71578512739e040702aed394fc18c83b4deca3cca6f43329fc5bd11b8fe0c96012c566a8e123c9681bda8e7d414190a2f68cd151b8ad0cd30875e71ba84ebd8f" },
                { "cak", "dbccb1ab429d42caafbb4420b7bb49d4ad31cfb06efa127d71b0c2fbe7922d440212b5b194203b8c237d2fd0e6da6cf3d4a96f01a6d508efb8db260d6e8ce41f" },
                { "cs", "df88e9a3523805c284f949dd135901f5cd7c69392cfcea514076b4fe3e0cc21223d8d9e51cbe8e346ebfbbfdd162de6bf565fb28b18dd6cfed56c84f20eeab33" },
                { "cy", "ca92636a0b43e13dfdf50e84c0dd12e9ebdbc510141ab6f6a1fd1fe4e95c6a4af51ba2a2a08f76c8457d56a4cbcaf0d095b9fa763b960ab6ecf1b8d96c889ca6" },
                { "da", "9242302ebab6523b61b0acf25049b8c0f33e84e6600b9dad2779112515cc7cc0c51557c62d621bb72ec1802605e0bde6943dcae73de8f531e3aad21599dc41ce" },
                { "de", "a9866eb7dbcf08fbf74a48e227f19ea7ff9573a4d7ca047ceb768ac8cddf750fc1b2df8f692a3ede9072ad65183f08b203756faade2d90b447a2a482655376d3" },
                { "dsb", "1158f6d157a4297329796915247d9a784957a1bd289ff8ee11c0d43f09a1ee5a37cb6a368aaaf7c11488e1144653f33bb9eafe77667631058932481f56b0d523" },
                { "el", "6d4210165d8c033ef1aa0c6c3c4f888c06e90367e3eb706dbe7d25db20ec542301dcabd0c2659721524e3729737b4bdda7464456f8c36a9a0b4b4629dd5fdbb8" },
                { "en-CA", "f0cdfbd4cf2623c7c7bb72c794ee1887a728ef999e181f8e2dabdd144d00ad530d0643ded781927217c6b271f03e2947c1d382b0f83120cadebcfab1d770179a" },
                { "en-GB", "5b47883dcb3aedca843c0c3f22fae5c69b1680e159e207395b0fab2597d8df700c465a89bc97ec7f019dada9f6612d8e349611770c0b9aebd4b0f9fc1ce51e4a" },
                { "en-US", "393d561f301bddfb0b2dbbc8271baab3353bcb0b2efe2ce4523d165f100a3c05eaf0d4399e63a4d70bce4430bc2c3a4fa4e832601db5fd1ec666a1290d37ad67" },
                { "eo", "05e9458a934aa7095c9aa3a5a211cf66ae78df9a0580c5e2b7406a8da007a924c12a4da79919398af2d06d47c73ee0dc59e0bb909044b0261b5d6d1b06501944" },
                { "es-AR", "b1a9de8a2ebc93bc66e77eacac4dcace717cb14d4b7750a51fbfa8b738e83224c9967d03430533beebed8e9b1861485dfa3d7a51df7de3516444a7a4a0ca673c" },
                { "es-CL", "8701b01505f730c1b298c44502f60ba96c2b7192afc8055e5cc0b1f7d8a998855341eeb399b448555ae028a34c3f0a9e6213da906c342af1a5840738d99d7b7c" },
                { "es-ES", "256fee36daf182cf8211db91610cd79f6e760178fbf1cad2dfd8569d107b73f9577abf69d506a95077ee705cbf19cf8859595ddc592d189ea9412014da45d8ea" },
                { "es-MX", "736aa454128ce88cfa65d2eb9068ca11161eb3ac081e4090349f9fbdc7baab0946b16ebd2c9189575459e3f5e74a11ac5e8f1e4a200b7bf3414e926941d60f87" },
                { "et", "b681e93465f8493335cac20e3d9309191d46a5d30219048d36391b88bc9d7f76ff99cefc458c016e87be636dbabc6757555739aed3a90af7f3e168ff23b96f2c" },
                { "eu", "c64e8b31ff84bd8266ff55f93b509faeef0e6ac0525e5c25c6236fef885f003d26746e7e5d0092bd3b934d0cfae7e8397d117350ef33d49643f5c002c0a30dbf" },
                { "fa", "cec5e93e23aecf9e2204bc96f332daa366c473c190e30533be06d62b4d4c7ae091d67509ea8ee05a00b7fa9c9cc2ab023883402006fa04d0c41ea1a303eb3bb8" },
                { "ff", "770ad13086759862ab70a19d64c58540062170151401159312290a0a8dc7f909724175850b36ac441459120b91d2ef3c861ee7e4d272b407b1f7ea85cd1fc8ba" },
                { "fi", "3d2bf1a9d0ea7065a8856904bc29084acb9e28a38a7f4f9c64ce8982de41c65c97c73e78ce77f7900a5f59987309ec90c89a6abcbe7b142df43ef74dad0ad4c0" },
                { "fr", "e105a9a968f9892e101caafa9465c3d9d9bc41eff5f13f1c380594a224f96139332e8762a6cb0e55d417eb91b593aef8861591e43d1c27afab8ffc952b08f2f2" },
                { "fur", "cfe851fbeabbb5a1f755b6a200c7c05913f6f6df72382df726fae4a3d8c22d9d599bb6abb0f56993dce6e2b7e3a839e2a582dd6a00a9714a0f48824c7264ff80" },
                { "fy-NL", "4a5e00767bf13e619752172bd47880dad6e9105c93ba8b311e615be4e4c94f2ab6a40fffbc49dfe9762639fd820d5f9de5e7cf0180277b8b169ce599509a3f21" },
                { "ga-IE", "999feff4a73d8224ea326c737b3cfdde33cc57b6f149219424dadbbe0b22088e28b8cbfc747ba75984aece67997c4b4c50f77eb3bc3a08b165f7af79ec982c23" },
                { "gd", "ffbafbc70bc14379379caa300c89cbdd951f788ffd334ba24745f69a836491b40007dc2ed385c560828d3c54ff4df33bbc8dd7f303f544c5ddc39d7e41e33bcb" },
                { "gl", "c5654da61f4b168f7e1b84993facdbac7269da18039d5dfdb116b3cdc0fdc1f1e8a07151380edabca3212fa9682bb76d69d14a8c6fd7925802b5ffbfb6e34a23" },
                { "gn", "53994e3933c469af79051027a23efb68017d9e7c2f03cf1efd98312a5c5df9b16b8c459759284d2b11548d17bf15382d57a457702bb0fb7dff8a7e43e46df569" },
                { "gu-IN", "fea3799eaec4708707a8f7ad07945713a52fe8907f74c2df555e780235ed9b22cf334f9a11dd7e8697cd40521b69fa9bdaddfa5cd1c6a69a4fcb8e8ca181950b" },
                { "he", "6fe7fa7467bffb34a3984f516079ac1f2b19bf6ceefc6bfc9e73f7de1173d3d82f6de3e5d15333367b97635e5934cd2afe79be949ef739c552a50a9ce21a99ca" },
                { "hi-IN", "107b053721d3718c436f63864f3439285bcc32da2f0cbfa0fb4f018f276f5c6f0cee4d56c11ebcbf62f6e31b42d726a652b659bb142ebf30f42bd7c5f81205bf" },
                { "hr", "d82a5cebe0f3ee40630bd3d5a4b13298ba86192fe1021723d89ae13c9b13d09932a7da7fffcf224db857537e2cf208d98b9a1c83c0358b9fdcd0e25b10131c76" },
                { "hsb", "d02d6fcadc686edb03df1bddf85bad2b20fa1da3206da5df38c79978a1101d6ca168e1a9cab8433525126b339bc525510224c5fadacabfb67c2bd406a92e03c9" },
                { "hu", "52dcba00ac1a05647a4e8e153502fb76a3c5d4ee6eafcee987f1213cdb39d405387b704b1f0ef9f910931320b5343c68a6acf40c14dd7e3b4b11121f9a22097f" },
                { "hy-AM", "d09df6b6055ac7f43824c708303166b86d8a7e225e9f3dbc76ebadaabacdafd131e505c85e5f870ed6646c4234e4e9460b0dd6d4ebd99fdd83611148632950cb" },
                { "ia", "af9245d267d81bf985eea3e3c5e6cdf0ac28eaf52847ecba28f81568bb633896c8b3ce47c1a13198a7eb0bd337835fec0e2d06dabfc1dfa808a739c628591d72" },
                { "id", "5b18f880d57f2ad2167b441b0fac3b92c3f136c54d01f3c4dfbb30675208bf18b3d2f3e0f6063893a06380f8d927be88488fd3e8ea92c99d8a048298218c13de" },
                { "is", "50a9b50cf2bd871b7fe18db4c22028ec5702c5ace2d6c2ac686cadcc5c6b0bd3a2a3f51dd51bc0fbd02e05c759dd10a1ba6f3a0c83fe03e06196bdd8bd1291cb" },
                { "it", "591ba98a3f02b854661d7ea042bc2dffed3a0cc6eefc8b9683d59834e2fb017b73918a3fcafac8c488de4b2c22b7fba033bf321f763563c3302a35622958a0fa" },
                { "ja", "ebe65a15a0b35ac2d21b77b0b1932b24567cc5d5d94441e7dfd0c576ce484d6dc2dcae6ef2181970769063c3ab47d02def44865bfd8de5abb611e23737c63f76" },
                { "ka", "5a30ecd11f4ed46a72b47296b5771f22aaa120b445669843aaa46c707d055341bacf12179d6a89811a5befc957ad5fb6f394b904578635865803eaf338d13c22" },
                { "kab", "f05b3a3967847e450c593267a1c671fff1649c62ff77dff0feccab3bd2c5cdc942775f16671b81c272d0241768c527b371e9ab20a82ae25a33da2aaaf305a1fc" },
                { "kk", "a874d6435a48c2d3113a18e0111a0d6f1d24cb2583624493ac4669436cb0c12cf601f895cefc65df658c18c919fb7b3fd6262ca0a5a12bbcc0f75c7e0568a55e" },
                { "km", "8482db4cbaf89d67e7178f848cdc1e6d97e6859a4c6fd630ead5f68609c460e7a9c110fb8b5bc89872664d20ca9ff00b2b733b7fea7327fac7a8c5544c282cde" },
                { "kn", "5dea82bb1eddae1c7d5de6df34f62a3f9a0778e10b7882d6ae94005e7513be0694102957c29a09074f56f13350d2ed1af75989608bc5f65bc010fa1d0e42c470" },
                { "ko", "d23e3916a255f236f0cc373c4193ef1cc267d31b4c35a34691af9b9b89f45b2d4eaa24d70b17f768a1b2f96cf3e753499c87838ec3f8f8bf8b08e58e6872f6ee" },
                { "lij", "5002ee9d12a435a204ce5d831fe540deeef3a8e477551c7d8c3d50a655e3692162e57c02354e9e9f84e82c6a3843a38b93c35154d9f03902e6cc5419be93ece5" },
                { "lt", "2278fe5b3312c9992f3fd3ad38825aee450f7edf58a33e9590e278f96abd87616fac7c6cdcdf35d18ed9b1e31f4bf85ddea1012362862b4712e92d83646e63f7" },
                { "lv", "1bf166faf8e2580e205d10ae30697bf3df1292d15a4c9d1549ff6a525b80b4e2f367899cd4e6fbf77a81ac5e526752ae8c69614b4af09f5907b3a8eed887708b" },
                { "mk", "32252c373dd3163ab03be15cf135352b3f590b086b52e5fdf2b8278c24fb08ac8f766c2f45d009abe683bf7fde3121278b2597e98edab9c5d0af025930f9963f" },
                { "mr", "65543624662c6d1f768538bd2acb2c31163cff4c54887a7aef601ec783cc66961ce65492e5d04230c78c2e7d2c155cb6f64eb5821a483510076084dca66947ce" },
                { "ms", "dc22d9dff5cdac8cf6c2bb440acae9ba636648edc3370bb1749b2fa152fd6c42bd57e92e3620d619ed9e5d7b992c9b8c6615b6ce74067484e2e2fc62767cb162" },
                { "my", "84c35cf1e5e9cecf47d0fd00422932e96b863d1a462ba542c7629d7c7bc17ff4aedbfac2bf578145bbeebe9e3f0d9e7587e3df807b9725abe153406b78c938be" },
                { "nb-NO", "43b9a9fae057ace8d6239753ea50a818c6a1a76798d09b2993e02eb41d24d3239196d7f3324083edfa90b69ed2d02a27feee5adcd15a569dac3ef3dbaf2b9eb4" },
                { "ne-NP", "72ebaa0ababbb4f6df87d57ae340f39db084bf47c3a8a1b7af99a3913ee218e1f55a9c2201585b3737da0c43656c64c00930f7ba9237de3340177267999e4497" },
                { "nl", "3c516f862a22387321433682f5cd28770fa69ee7d23f23d96cd36929cf5e0728c790debff58746d6f73e5955d57c340ec26ed47eb393be003fed54994b07b0eb" },
                { "nn-NO", "1b86c7e39a5ef491157e6075878d3874d9eada508cfba3182cb11c30ececdccde87fb9621f7f0a4d50d5e67f5b7cb81d776b76d7c4fa64aa298209be4c1ac810" },
                { "oc", "84a5c5c3b0c357901e2bd30fb56aa6307e8e442a039f65ce654bace0b7c1551055a1623c309d30a7910bdd6cec416a319ffc62555894ff683f7ea42f6172f668" },
                { "pa-IN", "a0eb14fe069dd4fa1fd36eb9d8840d0dd6045d419fe80a12e6e8e6d91da22078bb1668d81bf421bf17b78fe8bd8dfe30ae0a2b08362bf2e457caf34d931a517c" },
                { "pl", "81ab32d75dc19559d58740f161aafa3a458a6abf0051d830cf2f503ebbbbb136f9a6342044e428bd3f7e9582fb7e598d5c5ef1d994b1cf94ff8f606174215e47" },
                { "pt-BR", "8747c4f156b0f1bda5c0d257c4e664a115335498618607a1c7a5876b3ab0bfcbec98dc67eaea8e561a9ae3930997fb8f0b2e08a1c836fd2eb72d3bf46d2066d8" },
                { "pt-PT", "b924cfcc2eb8753bf078f9fe449248a3a10650e99785bd7c531f5e8777c88f65e0d68080abe9be973ceeea465d0bb75cfff2df8ae1500a1fd1111b2b2272775a" },
                { "rm", "696535ce75b4105d7c414e9c1a7fb66ac789e0f32612e6b0b7f7a84a3ebd5e0f33cd99ecb4e1645f1852e2fc099c7899ecc42234994b571ff41bf6943c958ba4" },
                { "ro", "c61c0a095c4d7df6386c2f44e5433149233e92c2b88535f91e08a0ad475948839da45346a681ca4908c964a2cde18f510d5df3ce8000c97d8fb3848c54a2ea3e" },
                { "ru", "1db1774e83ba72cea5ece4102f9752e0e18360b8b8b6252d3937e830683359a63c511524c9c908b237d1361855e655f4ad0c69ed27599db4ea8a5a094c371edf" },
                { "sat", "f1f4c54c483298775f99be40ce30ece3fffe1698cb5e3b2963442e1a314520cc9ad74a812228772d724de9b090386fa314243f75ee5b98e59f21d7d4ddaba303" },
                { "sc", "d9f2641571c5da90f75e7d2b1ce1f6f4125b981195d0e1582981ff5cb7dcf96565e5813b5bc20107953cd29db0ac4174cff799d1bce2e6440b045465dd2eb9cb" },
                { "sco", "d59ec13ebc543205e692622e72b25708418e0f611c41617117eaf580d2644bb20ce1bcef2df0a25664ff0abee0b1f72aabb6b6f8a7dc3ac6d588f49d3bd1c13e" },
                { "si", "02a6dc552e17f38f99e7b8763e91b130a6fc3cb19a479773ddb12ecae7dd63975cf0248d77a9534d458c9dd5ee5fd9f28d2900df631fc8256e8549ea9d7d74cb" },
                { "sk", "7ad8c1d45e3b8589a70d927e33028365f82da883a125d0b9e8eb745f4d3c9daa428864d525a2cb019889893bdb84ddd3336eaa8f7bc8feeb07093683f5113db7" },
                { "skr", "dc90ba24a7baeba776deaeba4650cd54664c9c37dfa0073dc520ac911811cd8013b0e72525efde4c981dcb9c8b78e76590c9f57cc6151d18b9600099525fe978" },
                { "sl", "c8e95808b999ee83a42d5fbe6c364063ed927fe71e170b9cf186144fb837c5f99494a109e22b70a74092d3c4ace0141e9145b11fe03018405373b981ec224247" },
                { "son", "9042740774936979c38e36e451c1bff0baa01fed3559c86adde7a0ebae366bedd00541b7d7c09299ef3fabdef52dd308d31edf94cef4548a039f78c03e15b410" },
                { "sq", "615b0a22490b92c6894c3a6561a962ec7507307dbf69bdbaa1276fa1ae57eb5d9bb5abef184a69b067cb7140a60d0443314bdf1c8bdd66a6832149362ceb9247" },
                { "sr", "4a2ed7f9f2de4007445ee1375cd697e20e1c4113368c066cdaf29f00a73328164d8426d7fe41077f7aac216801e4fe76e5c8da3fcd7b5fbe7e3bed0d92d2f4bd" },
                { "sv-SE", "ab2696cfcda21bf577a2ce465b30a52332a30d850a37b246830681faa5c0920e388975fc9725dc8b8867dc3c8f945cfd98d4791d01931f9217981ad321bf79b6" },
                { "szl", "3eab2ba2848d8056cfacdeaed196af3f535a748fcda3fcdb549293315e8c8ec13904bee5141a7543af790eeb9a69b3873c3f9b1bde3834c1c9185428a7653e4e" },
                { "ta", "07008b169db6fb4b67e97ef5e4ea630fc372c43b63fae116e6e7f46b3c53c9a5eb660dd597e3c225448cf87a8bf5227f818f5bafd8c58d72d93d6b3f6501ae33" },
                { "te", "6b87f889f5012acd9b03eedcbbeb3c5088d0f0082f044832ebe94f6f0b9aa7f683e83acb69ed8ea4941e2c14f1607da604bba31f8c8f4a5f1186c45f7014c455" },
                { "tg", "1e00182806e4e6bba909001c745ca1e88209ff947e3446d011c4d809d04c59aab55108a4a14162429cc7a3adf75a3901c63bb19b3d0a8144347b837c4df290e4" },
                { "th", "362d3c1dc78bb3a6545efbfd46ff87c8779607c0f7df4dc9e47c810a8493aae23544eb769d21d21a910277206debb10a993666361fd0885315291d1f766caf93" },
                { "tl", "83b2ba29ce447d4db9389ffa563e8cf82e46a804488cc1431e2428f25dbb6fd7ca2c06fba602e9c1867e5232ebaf969421e21f262f5eee55684fe44ea315364e" },
                { "tr", "71d23064f1fe1a601e805abac1bb97261c27d9f1b7151e6fcae470ae07a3274be733d7fa5e66fcaf789e10f5778d63de826aad6837090f623e83796f1d1414a6" },
                { "trs", "c8118802437410999dad29c55af834453175413bad5e4508ecd147ad30b47743328c2a608ca51282f6322136ca4d1f349134521933f79ba7c5e1ddabb0cf2881" },
                { "uk", "63877660632ef0f278b50cfe6ef650defbcb189a33feb8f14b981cc22b45532ad2d5be80f325a916ae5853962eb0c29b81eb43bd7aa8bdce56d78b974a1ae2e6" },
                { "ur", "57bf2dc38ac1c44385c13eac1e8f8b92fe60fddc20b371599dbc656fa5c50e105cb926787b673c34dad2aafcd5e52cd47f7eb1e3589bfa9afb788e5f0d80c178" },
                { "uz", "fa36228cf7f35060d06407b7c01aa543b2547c1b0c43265e5dcc73bab545d71ce1221333c97a782fc4fb1a3849da466f62c50524d42101692bc0054e828ef010" },
                { "vi", "bedd68217417c4e92bf8342d5174a6271dab9f30cc0ede5531e3588f06369e30188ba8557f486a88e2cf6376bd7b0490ed02ea02402209c595aa62de686b96b6" },
                { "xh", "f39fa867cc9ef68d69b5129e114a13521c166c127fc3fc3b83bcda9079e0cde7e921cf9fe79c6c8ae41b521308cbd7dba6ed6575a620ebc5e2d72582cf04b6a1" },
                { "zh-CN", "390124ec333ff0b17442e8ebea8a790d179cde57028bab92c9db98e64600db9623c95a237f82d9bb57248827dc5ed0278decfea98ee6adaf03a4fa9783288d0b" },
                { "zh-TW", "9904f4383023361cc50a6d7480340347c90982673b70aec3eabb895354d28fba4eadce3c6cf47e4bb101e00d0e39b03322dfa0c1872a571ff13de52310719e58" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "47e870f974bb8ea93144cce311937d71fbb595b19f649bcc9ca3e403d523ef351d7e653972fc8ed94251ff948078c634cdaef7979e8ce0ee160ba3c35755fb32" },
                { "af", "6e3b4e34d2830599f9c079135f49fad0768b34c181eb5cbb60770f2a4c30ca3b835186b612144cf5ab438d3fc46fc4c1d59f489a618a42895ff6c6d87086bddc" },
                { "an", "219e8435c9acda27cb3a1fa7404e9ffabbc751c30b67ff659fe5b31ab4d61fa23fad94457390cf6268a4eb0130655f02afaaf544586d08ebc2bc4423c1e9021a" },
                { "ar", "2ecfdd7d4882dd4890c1b6097101db6a68a87fde35f161c8743ecfc343aa2c450ca3566955b7df8ac366494b8e318f611688163eed0b46202429a1ae84b8e811" },
                { "ast", "f58e2223b2d0b47bd1b812df6b7c5a021ed9111055a193b6f754964455191360d3c02a58f4e09a6b87de08408d1bd0347906b4a466c3c08cb242e24a636a6a08" },
                { "az", "af57f3e6a7629b4d765121fab61f3396c80f724911cfbb7d79d5e249f993393d07fe919487f1434bea1ef522a2afe36a884d2c9f219464d118e019bfe229ad73" },
                { "be", "95046878b2dc47768dc316117d536dc9093c2f71f7c9c2a6a2ce7d958d4898f62c95d424954cff459be9833fbbd95479ba84c8ed59c3c81730ae9a1b02797ef3" },
                { "bg", "e34a01f83bf6f7a53da8f64961130d4945fa5021d142c1faf7de19f9e5e7fb6b16150c05f07c0f4b902d9ea92b4f9d54ffae65acf8a3805b5488b68f1840dd2a" },
                { "bn", "eb41de1015dd679a77a55526ab51de191cb7113da65343231d06a1588582c3ffb5ec681a7f6c5689a2e212b186c5db59ade9374dd3f743cdb3ed9e53ac6eed9f" },
                { "br", "30a0573cedad340ac71bd93353b71f6612a789cb53ec6bb3dc44fe6f9e7da23dc08667eee4492edb960adf60dadfabba13093668063b214b1a7e3257d2b5ae64" },
                { "bs", "b3b5f24379b231f12df569788e8acedad875b78d0c3bf7a140ac91166305b51d65d58ee8abfbcabb859b7b5345118b78c3f5fdff8908dbe0c16cb1937e0d2e10" },
                { "ca", "4bf4e0cb382745726f5c6e7edf3d8c5bdda734cb2f663bd7a189859e90da0df6ba8c962af55c8d3c935174a6bd1824417773a6c812f39e2f8b27a626b7ce65e0" },
                { "cak", "a4286223245ba4e2facde69d279f9e90e5eb98c7c8cfd2781f729523cf53bf0f32bf3c0a48e08f76c49e155ee65f3426e05951b57d7bf44c27a7f8a4b8b89055" },
                { "cs", "8c76cef7373d0fdaedeeece2338ad236529977965f3887d9631353aae7c972c3c6380364b0951d27bf6113c96d193dce4d1742d86e59db357db4583f5e18abcc" },
                { "cy", "0c298dafb2ed82f8c51247afd4826705804fee19518c87d31512e4075233515234d21650e1c661e4d54cc04be45e98660f34237bd809d423ef3d73713b0cb9d6" },
                { "da", "7d7f2345e823d125e3d1be553ac336c383b3303939403ae39f16d355c01660a1bfab9f5b4c4af76da34e4b392971cb90c6092a3b0908c024e1a809bfdbe7858e" },
                { "de", "276aea4609fa91bdc3d75d57bac38e2f204a689f54904874fee9e8652acc5ae75e59f98fe7dc77e173cd213c82292dbeff183e40d7c48d4a1bbfc353b43b4552" },
                { "dsb", "e076c2751630dfcd6fe4511c49d037beb481dfbe918e83b60154011d0d678a3ebb7f794088f482c73300af3df54ef6c0cfc58979a6890e4207481aae2c0f226e" },
                { "el", "d92de235caa82d11b0b4fdc0858938a84e1f012b245a5ccd95eb74d8cc748aae55a0b5ae76d3130a9b96c38851d86386f244f3551ade7b9e2a363280ae1eb48d" },
                { "en-CA", "809f847fe3a233a65e8770d63b179e946826d53f36682f51e5709b275469988e99ab192a550e341117ea89bf670ac521ef99052ec6c5053e1cbaf0022fb6fdce" },
                { "en-GB", "4c497c338b3e2986fae98925d9460ac2d0aa625da0734b24d60f3f639fac0ee4b7852fe62aff72a21d27e24d8c51a3b9ecb6cc826ea9cb75945b28020c9ea309" },
                { "en-US", "e7b885ba6e1760473a266de62529c3a105d6d5918b102b2c74481b7300ec088777688467255998fc9e7710139a260a83310127519f85e5e228eccd87cdd57141" },
                { "eo", "dc9be0e64c7dc97dde5433aac3e8019bb10747e7bd87e744fa9be292c58589570e96b5725ea109ee08423a845720f2858b1f56e3a49819bf42c0833366b8900d" },
                { "es-AR", "4b43d918df863a06755e1bb93e2255bec5a34a171391afb7d3e3baecd426ce4a3149bef70afd90cc6244d7152dc7737b9a1d06bc0a8ba9e8be6fa56472e134b5" },
                { "es-CL", "f97babb380372eeaf8562d8e756256303fa6e1e920a937aa2e217f89763e8723e9d99bb0bcb90c8b0404a3068c0dc5204cac3d858c849cb4afe403943bd8f9f3" },
                { "es-ES", "8967961b84897b7434628828f902e1c4f7c8edf4d1d26cac2c6545f311ab430d8f998c34680bae78da24f554579b7ecfdf87cd5c51cc7584c354ca6159a9e8ba" },
                { "es-MX", "cae1cbe9adc85503a2f34b9c9d08ffe2e1f6fd93af8687948d3f02815d446c113283f46424f7672773a61f28a619bb89aef9914d1d4ab98c106eb7d7c3367c35" },
                { "et", "49c207c25bae3f541c3a1fc255879fd89d46d337ab0aab5401fe702697605c1477b9ffb498eec60e59d91dd681ceac17f486bc1c8b60b55b4049cd37c8f0af12" },
                { "eu", "6eb2518123e7c2056dbe9bdc152852db35555b259573421a263cc03a6cf6b999d02590202136b25a5ee20ab27f7b3819a8b63dea2d28e7b3d044a82831ddb66a" },
                { "fa", "d2fde460acbc1bf55f4f0ece2912c247d9906a4b8abc57132d0d8487bb35a29c386d1b368b1872e2292099cb58b2a6e4fb865a70755675fd531d4885293c9c06" },
                { "ff", "336f6927ee3c9cb8587c16f3073270fbb2c560047912bb4b0a500a5777588004491f79dee18dd9888a6540586eba964570b2917d91c6306f3523e1552c503d25" },
                { "fi", "0b305eebc8f6a42c77b272086d7bae9ed2d683675903e8488c9092723efcee97596a728f2aec94ab869debea605648fbc520cf3b845d3f926f717f053ed12655" },
                { "fr", "b2e165426576abd0b7bf0061b799beec4537a4eb6caec715ae8937ae218908cb8a63ca276ca61b2d15ab086289c22df8ff5ebe23118d616f73fa873004a8f6e8" },
                { "fur", "85a9b64751cbb108be2afc1aad21450eaa74ac3420e48ab9260d9296e9c25721d5528cac423d0105ad04d76ed33b90c3a57a80fc74e8c8abc31b3b0bd3b7c8ab" },
                { "fy-NL", "fc039f521d1786c14cfca526fc1ad4031e7a37a17877fd51d6e5655ae8409cf03418115dc4a9fe5f70f3029df7dfea33fb07ee86f76b2529cb972a53e398ef19" },
                { "ga-IE", "612c4c513196e4baa1086a61cb0a2eebe1f7b1fbec666b9af6c58e3ad8aa119a62acb3c87c2b49898e35fb83d594f855c13c25cc753a5afbea0e2d9b73b57b8b" },
                { "gd", "f5d66ca29cd32e596659928a9589c78574d8b84e1159183af44037d43449d1af0eae823c39a198d0a7894138ea7057209d5a5658e5fb6fb0cef9025bc001d10a" },
                { "gl", "69d3aea78a74348de5a9627437f95f24b63dbf7033844f9c09699664b7744165474cf53f4f9de6c985cff5a833a638af62df92f5999a6cbe71ed1b178e554b2d" },
                { "gn", "e59ae60fbc73323581b09abefaf88b52d8ac33ef510eb9af49d40335e137b834673d419f47c016a6d304b3b03653985fdfe87a009746d655272255d937858415" },
                { "gu-IN", "b3fa394ac685a135746e4bb899da0640c3b16180ac26c16488ccbe5b95b98f6903e18420634b09a624c19f197eeb76c9dd935c308c0dc7107b0d93818fdddae8" },
                { "he", "45c6db1122cd90012a59ac7a7e71962d0deccb04ae64019a76fb63293b1212ece789b93e76607e466e03a68599ad6129214ce9cf373a1a03fbb21ae3aeea5d5c" },
                { "hi-IN", "7aab36eb493f20179f27ee140102a62889c1245316e70439c6abbb42e6226dcc8c50eab50a6c11f46013a330251c0143aeaabd07af30d8e90b7de6a61ce981be" },
                { "hr", "7234fb7abd41a7cc9b0378ba52b01b6ee049065bc0d7695cf35fbfae97401b82464b8bcda268f58912640ff08d894ac40af3b7fd0867b9b9f6e45bddad81efd8" },
                { "hsb", "95c49d10122cbe364550264d799ec4b5df2a91722ec62ce2478a64a10fb9159d036d5d75264f40cf855c0a0624e88b995577e11e6946aed8d03328f166e67d0f" },
                { "hu", "f51272f5b22bac3526547c27d2850770cb0974e4a47a5c3d1ccd3133e6a16992ed0c296944f38e6e0d9fc13914fb10fb5edc0a91134b0fb95be9320910cbdccc" },
                { "hy-AM", "9644e3bb6b2c97c09078eb70cd2f14603587b2d0decee523cec5f95bbe58b51707a4b83f36230e0290ff7ee86654a06db84d8948d0506582101af77d876430a4" },
                { "ia", "5c813be7f2febe92155bd9feebadd601ec565aaecd0c8888a9bae7db3ba56e1e12027533328a4890a61192b0e6d3609ff713c43b05ad577a5407a7069014e4ec" },
                { "id", "4118fb22f69c20bd7f38c3c65ada588dae01bc139bd108698b066f1145316aa2c7bc1e1dc32f05c566776f2a98b25b2dc4d7a91ff9c76bcb8f4ff769c50f9b40" },
                { "is", "0bba5272fa4249fcead52dea52a9369850095da7cea9d965894de0d1e1991842bacde99aef4dd3da5e38fc010fb98974c8e62d452ac3f0e1ac46b1ff171148c9" },
                { "it", "526fdb03a6452e1a8b7fc4491bdc2a9c3b8bd8f12688a0da496e60543482acea63dadb4f8e913634fb39d5391361cbb391cda9ae8b6785505dea74c0cdcc43fc" },
                { "ja", "10d92e144af0b9c19dbb34898cc47ed0ca240a8c54831426ce7b5193be952b86ce7fc0d33fdda54c275c85bde986a80cf26c2fda11b9671f72a9da1393bb36f5" },
                { "ka", "c8d23f877de21e3bda8a6c8f59e4d4aca6f16bb12e74ece7a74a534718521c45cd0f3cf4ba8e5f4f53a6bf63463b60ebc844768d6683e6deecbe59716757e3ad" },
                { "kab", "0c48ba245ca352152e7de0766c363fd308698852de207ee13f5e3e972b065be0f9bde4247301728a4a3e53c6d7b049d96a362495b724e77f75afcd84d152c2f0" },
                { "kk", "1353a5c439ff15c99970c6b1b8bbf64ad8cb9b32bfd4f539a96ec30a82ef1ea746083119510c98c29ddc57ce89c9713a1c3be36d451b4699b619e0f1475dc5d0" },
                { "km", "1f812fe2a4a7d4f6d06f35cefd9feafe7582898e8cdd40263b60ba1f50b9f5e165d53dd1cdba6e13255fefa824686c00e37710d67a269e4c5b4faa937ac5e98f" },
                { "kn", "6942077ed7d6e844e0a8b8f77c94c82fd7189e820dc33665d3f41da15613b25e79b4b8dcc87d6dc7348f0c63ac6b5dc5e74689e77b21b791a613ab15bd3be4ba" },
                { "ko", "f01feb3de370d2f5574c181657e340d38dabf9e96108dd463cc7e4762eb366ea7394d958c6a1dc1f4eff1110049c814919bd4a054171c727d588ddeed289eec7" },
                { "lij", "d771bea66a0b244254dea4e4c57a3348bb50c0ecf19efde74f2130b4a4b5e08fe45e93e1203dd6cb5fe8b7156e3aac887a800c29b26b4df6290a1c851074402a" },
                { "lt", "5d15bf54dc582c336fa5719fce36a2f723bc17e37834c204de730911fd647a9fa5e9d847221861482a4de1cb525977c07349367b74a60257f5d99b63624848da" },
                { "lv", "8cb39ec586bc29fde1f54812a1f8695b4bdd81f8cae4798c9f8d1080ddc8b53a5647fbf2c83f72a724758b37acfa0d18d9ba6a980d2a51721265942c10cf6e01" },
                { "mk", "b8e44b15c754d9ffd213d52ad33690ccf2259e4c808b730ca70af14582a0f212e714846ca31c10b02eadd1e9dd7cbd7af287bb51b48f676725d5f3348bdbb0ae" },
                { "mr", "568d720709e3f5404fef8a625b8deb305f73d0380ed047dbfda1d87963d461fd1de25d359c26309a0ab40511f7ee83a9d487f5940399eb8d4aa0b49e49944911" },
                { "ms", "697945193061a1bcb58fca18fc39fd10de76ab8887c5c86f4baca2832b06b41faa806e611159f002cc31dcd861a56a7ff313e9f8fe0830b8c3e5047de9fd955e" },
                { "my", "e735068384d397cf8883cd721c6d933a50dc49845a3ad154047c5d22b526331e6ebd33dc7c02d076e7b27cbce3096f88e4124856175a8eaaa798ccce9b4dfa30" },
                { "nb-NO", "d523eb76907275350c7e89ef074be51353bc1493d58df3a5c6e2ea9712fca3c29ecfdaa5ac1ad13f4c6ba225f1690caee69dc8d742f160954daab34dadb7703c" },
                { "ne-NP", "852717df9fbc0c03cd9de60b6fc8fc72b39b22a6f2953352d7deb5695bd2872ec07cca161900354710c5ae447f794fcda9a6b04d2b1dbb540cfcf9a6d9c009ee" },
                { "nl", "d55857536570b7e880feb35f28312d8dd1f9824cd772786b5f6a174f357d0dd55fa61b6ed5032cfb1832d0b68c8ac6a8443e364724f7d00ff572c8c0228635f4" },
                { "nn-NO", "842814f131a235b1f03a1bbbfa95bfa5727254a058b9e4335d26b770f8aecfcf3553d46ca75003149f7554badfc53d2f7af6f640ebc4e79e0b6c49a57475f561" },
                { "oc", "10f29f1f2135741584b9ded09555114108ab36b745f20a19b10a1ccae30ea391fb92658fb3802e198609688ae1b66f494fa4b50d4fbe6198969cf3441d124a5d" },
                { "pa-IN", "fea39ff6def97744f0b9648b774fe522acec56bb1aa1cb119eb37c760585faa4cec61929f87ffc44f1390e79ff144931b0765fc4b75dc9ea9d73df0846e6d513" },
                { "pl", "84a895fa1a355cfa8e5e9f36a151c0372a8a64ce852e0f6c1c129dc69be082e925ffd7eb99014a91451a8404c5132ef3f2f5a33e4cf9d49592d446a571d55f5a" },
                { "pt-BR", "5e9f80b197c81a823611dd3c1eef393a6d34222ea69629988ca790274a4a835799095790af4ccc0651cb06a1aedc90edb189386db681152ee413ed84bd600d01" },
                { "pt-PT", "5a5a3d88c3d4c8ddf28ce198e5dbf4808c083ee55b7a6489548e5c520b33a27137635067c39a2394fe33c40a88c89b5416be59ddf71a3b9d14db24e020102324" },
                { "rm", "179b0b7b121e7fdb4ee724af13c425ab200cd49766641b5dd953ba00c9401d388940890849069352575adc9c11a6fcd92377ad3a73ebd0cedc69dfc58eba0394" },
                { "ro", "cf796f2ae0ee60b8677424eeeba730caa26a3ead4bb9cd1f937152f882baec261b34792bc8941913b7dfc617339a46338facabe99f67880a7a63b3637d4b6d26" },
                { "ru", "020dd4b54b8bda091f1b23735389468ede3cd93888cc29b1648adc4bcff3f7c45df653fab9fb5fe9d06efcb237969594600b71a8bb11f5caa0bc03f68502ef85" },
                { "sat", "2799fd9ed3bb7eaac5122dce37eb7f9856e54cbef28aea172aa289c514d023fe7d042e4fbedfaf6f37e1b6cb6b576d797e37257140cf5f4341119adc829daa18" },
                { "sc", "69e133b796d4fc52608a4db73c50017eb677a63a0950cf67ed9d1c717c2811afbc5072e09dd65d49b42db73cc060286763423e0c8053287bd15a700cd2a2d60d" },
                { "sco", "70298beed25a0041c3882bb69d46d58c766e86ba31fff10ee182237940448889ec6ef97e9006e93ac42b186d1a376c4a0e6458893c03d9c8a16f09ca3922773b" },
                { "si", "b5b48bff2b863d6bc485f4c9125b141fc6655be7482f737695f22a3de58155446c8a52dd6c84084316467c1234c69286412a3fcc7a5bec84e6694e9c9954e1b6" },
                { "sk", "fa09e93f24f72cade324e74547640b431dc314f62525c228c6b479083ca17f7bdd8e507f683d66a7d93ef8ccbd440f81a6a5ddb1b5342c63ddff19be06426f69" },
                { "skr", "fea300b39eb49836455f9b1c84071accc3eb3f8f761bfed468cf1e24ac3aade715de106fa3b61ea3f3e58b4316f1c75d321fcbea7a5139799c8c22a9ab5b5f18" },
                { "sl", "c6d63fd713c7887740cfdbdfacf3e6e34769b015ae26eb80e446dffe95dcdfbf85ff46ea55304e2ae704a9b568740bf8efe60920236cd69d2c8a1c69aa6aeac4" },
                { "son", "cb922e0be67ae5111e7f4f4c6fe4e8862d2aaf7fe8201ea52712eb65804224bf6e7e6aecdc5f1059e9b557e4cb3582c9766347c464731285c419bcd22749014d" },
                { "sq", "0f6fa4e5b8dca7eb581ca5fb808a01aac18a9ffe02915caa6f7da14b186cc720bebb37bbc41915e62c1d43cc7c0cfdc172ee4bad40ab3f9d7d4a724170ab6d60" },
                { "sr", "649e06420cedb4f0431424e15197d01bd31156785855bdc15ab3154e5be285b5e9a4ff84d74d6083471cd3c279fb2f93bbf42eae506c4933ed397a57ad31a44c" },
                { "sv-SE", "3d0f7f267496010af7df0b2258abba863b70ac9353ff6f680638e668c057f517431a64a679e44d3541e1eba55f3f7a258f4615d2c591a9318259c0d51e8d9acc" },
                { "szl", "cc832b3a9f51b486e3d3aefd40e4a9f8f4d5415896e834f36b8400443a1a1f545ccf3692384bd89549fb91d3284cd3c436c63d0b02d4fef7df42c19173f0ac76" },
                { "ta", "01301671070bfb85ca0356420647b10fa72f9c3edba7c7f95e7b63a7e2c8efff80770094116a42e45c3db1d6fd4fccb3b570d83e92c5f092e4800b34a29e0d4b" },
                { "te", "02a29230038d7f18baf5a54f28ab7b1abf7db7ff09dbabd255e0c7f47584e5811af395a458b02ca7e653d40277b69f46eb87be0c4449f5a1d141243de0a6efcb" },
                { "tg", "a58df99f9157dc2c64bc0003d78c0dd90cd1ac9c80bf90f92821b6833c36aa2c6c598738845e9271dcbb2d3a9696b360ea239b824c258ef8d9173b0f4c00a92d" },
                { "th", "56d386ce522e4e73242cb2130bdde51353343aa11ed5da8bd8d77d77cbf928011325e377e59d57009e7e6614074d95f2e76510192282c613b71042e80f37c249" },
                { "tl", "63618f24fee0a010f9242b3c54e8688f228ce1b119388e0f754e940d9c4d4248ba04a4d549596bb39a7866096457559cdaa6f10ff9261054a68f6e2aeb32649e" },
                { "tr", "facf4c262a59e68c27e38ee1faac950821fcd7eb96e255f2671cf81b7ab53c8783c842dc3a156e7ba727d698d9ccd775618f0a10dc0bf2607450b806b1d817cb" },
                { "trs", "101fede266eae1b111541b1623dafc8717b1c90321c232db0a0d66506c430141a1c42658be2275a3c658f9117a60e5317f0fffab56c0dfc35e8ffac56b5c1a18" },
                { "uk", "8c121af64a5b5c8f039b855fe6d3e6670262fc70badbbd17aed2e28d570fa0e4a622ae2be7d2c7d18d7df303f9890c7862a622f7a6c44052ca79ca33f76e81c2" },
                { "ur", "49bdd1b2d44cbdfb7cd7210ffc11882a68e47c5e3221da6dd242a81cd294890184103a17e29faf39e9817a1e73a67b231cb67be0e7ec204463cebc798d628cc8" },
                { "uz", "a25a2d745f95288f30e66dafed4881eadb19049384ddc12b87e8575bf5f9059f9ff1433bf85297041ef11a62121c77ab83a90932493a12893344c3382c50a983" },
                { "vi", "803da06387a729bcfa702e23348e0315d70187d2b1505904ded40aeff9907108a6578eb396b778258d4c9cb56a45cc4f34b645f90d21371c0eed3510158722b9" },
                { "xh", "422661d36ea0b395894b6671c4ea33faf303f92aca8a3e5cdfcc4f3bb7e080f34cd4fefe470236a1fa56089e96718ff358c2347aa25c26b23e160ee59ee47977" },
                { "zh-CN", "135e716b0bd58a179714fb8a515bf7eedfec99c183518f04111b6d931952e53a819069709aa6c3bbfe03e341f0c31f096f890c35a4c12f398347061b6cfe1faf" },
                { "zh-TW", "13dbb6b77087ca1eaf12a72820f8387c7d72d5d207a1b64b482752f08a5d1f6498800b2f6694f9f61ad51a602535e6d41e4b4ae6706352593aaea0ee2b6f3892" }
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    cs32 = [];
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
                    cs64 = [];
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
            return [];
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
