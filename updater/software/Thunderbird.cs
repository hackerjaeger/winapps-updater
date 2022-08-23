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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.2.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "90b47b9f2865f8bc42eb0c152b7c6f5b65431203242a9c25ad81f5240a3ed936b619817015ed9043cf783e90500d7d2b4f359e33f03a5341c6a2ff2d7e3d04cb" },
                { "ar", "a61b8df0ec08a59e297bf3aab4008dd66aa02b798a08cccb12da12e6c75a91b14ea4f8310ff1cf404fb905f7036696bca16ea1c5784d2bd00b66451419ef90a3" },
                { "ast", "ba8185a905c579c320a9396e5987a6495d29ead8de83da88908031c2e2a04ff7df685e31cf4373eace21b04f84e44576c7d77d3cb7fa3f1bb99dc88427c72ea4" },
                { "be", "eff12fabf10caa98e590008a8e3720bdd3b248e2536e9dff01509e54d9049aaed31232fdb43bfe12aeb845c805469bec2a295781444712539076b5b95d27116f" },
                { "bg", "2b07dd6bdf38b1179ab71724c365417c6d2e7633989ae8fcfcc92273505068392db93a8c6fcea03b8c85b2fee519bf3876034b3e8c4cc097c2329e2c0a2e4e4a" },
                { "br", "0fe664bb13ffb29b6190b4c84980fefeb151a0e78b4d010baad6b78bcb0ad4e6d3f23cd857f9590705e3127c44a73da38ff617bc2d194d1aff91e560315e61f3" },
                { "ca", "675ecd9fdeaaa81cd6f5dc6fa92956c29e942c78b64d88842c1ae699610cea6014a11a98bf51542175a90b1f01c6be7dc4d06052663a9a9fff28fd3acf4a8963" },
                { "cak", "be21679d6939cd42e2f644a6cef3dadcfaea76f8bac6995af8c8070360d7d9a826cde3f7c53dc1a94b95a2220ade3d3259b2a0d60f88a75a51ac14d299ecf0d7" },
                { "cs", "c63fa094a651ccc787a2ad1b8122ef6bc1d682ba0c8962a8585e04d57716a989a7b2656079ab6f2e40600c700301208f5f3b5f821091e665a3b91cac787476fb" },
                { "cy", "e041f927eacc8211e2a96f51276b759a3515bbdf82832c047636cfdba93378a1209154d82b51e6d3e42563dae58c11cec0bfab1b1abb774958a3dcc21c9c1ace" },
                { "da", "783820e86662aba8ba3c30a736257b2b2a682c609bd05efb86ea00d85989cbafb03b611e49b042fdf2cb96ab88c6261ac0772790726c38bc5354253814e0d8d2" },
                { "de", "920ea069bb0e98904f78c13e618a133cf207cd4f4b22b1da22735dccc29450fc8dd81185134f034449c59b2305039aca42b16a62f6323d0598fda39606f005d2" },
                { "dsb", "2e8175124527027eb6f4904ae94cb06e7ced114af0f92b11180c47ac2c5800d4ee9306f1508dd72254d9c64d62a0fb3868b4a3bd8d3234b783ee52a34616668a" },
                { "el", "1742acc5cd1891a13f2ae3c181d65c62a1968155f2fd59e25cb3b7cb4e014b0a70bcd31987c203be7d6412d539dc15e4b31ddd40a1db368f96dd1551c58eb177" },
                { "en-CA", "49773efabda020a013e70f76cc0ecd5ba8d45d660deadcd5b927da4a8fae1c5ed3851e4cd6050f169affbe54a869e605e3c61443a4de9189070874f4d5ab2c37" },
                { "en-GB", "5c5195f2087a11371b8dddb8763c46f3e663d248657d2675087c61a3c4d9a0b8ec1eccb3c8b06b5e78c8291d8a8a5e72fdd7152e50072cd77d85314ea9f8f755" },
                { "en-US", "a57ad9beb6d95652cdf3b2967f879f8282074b58fcbe33684a383f23600d2b8476decc9b71ea091da532776ee66ef36df5f9ef5b61957d14fab6d83da3c59611" },
                { "es-AR", "a89292d9af02c8c4e13f9e177994470dcea3c3510f0cfcd1af20915f8868e4f565979110b5c40cb1e549e0573c0749da32cd25f3a1b4b704f1a118190430e63e" },
                { "es-ES", "d8b5d3ce54f21cebdd83546ce56f0ef9abe2e1d67b4664528bf39be52f9612e78b98192dac28c31f7c42a0ef24575d4f70db943fd190f60d1d49a97388ce4c06" },
                { "es-MX", "3829dc2584a92710529610b91680a2f64420af79f73af0d5d200579fa98ff879a041479e07619316b7a2f8ed814e624a0dfdbec74402398a4ed03ab63e41ab35" },
                { "et", "a40c9831f005dc606516eed274e0f833c1cfa438ee1fae4d4b3711cb7a625fbfac0d2b8b211c6cfc4d84a0ca54900b814c72b2dd288e74747c70bc78661f7acd" },
                { "eu", "c8e0e89b1abde2809327fcc8a983aba458449b577e144015e2cb0170eec3ec9ed9e350a7e56da18c16baa441bc1c41402b26e8fb70998386af51f66de2efc1a5" },
                { "fi", "0c590bb724eb88862763fd2874a47a1e8344f546d518503da008d4c43e73cd99e7448f11c99315c7fc8484a2bc20c76e244067803e3f7bae2d830c79bd9e3a5e" },
                { "fr", "bd568f7ad6c4178860703af1087ee7b5dc9326c77bcc9653a4a09b8d7d03787f66b66c31a4bc128dc6d20d5b8e02e2e421a099569bf9b26fbcd7d69121dc8bb3" },
                { "fy-NL", "c4f0e14bb4108d7354a1cdfa3ef1fc384ce51985a44705b412cba6581a7181b9d36a1f452c7b5f061ee03108a65ceffa29d5a73476dd6563ce20639c2ce2dcdd" },
                { "ga-IE", "cac8cf07532a39428247a3358c68bffb5fa63e4067ea11f9d81e68513fc46616ad30fcdf47d39eb3e1fcd2aa9b985213b59be0fbca3c2bae0c924145e1e134c8" },
                { "gd", "3cefd68045b03682a823209c63b8d25b125ab80b8a8eec3202f84bfed3f3ee73e8471ab4a4c90ddb87bcbf724e0e2426d1c6eaac2da536aa83177762bae7a508" },
                { "gl", "3623e5e2469b4cc1ea2f5421d5f462c005b28adae501bb04cc25fbbb44bd4373fe8fa3f852f38b7060a145dc9a6888541feb44a7bc92105307d94a21d5f181a3" },
                { "he", "387d707309a9ab8d597e661e56af69b898c032b6c8e6134ba46c55e0a52e31ff2a147cd7bf6d4ed5fa62976482667ff5aae8aa1c311778d53a09958d464c931e" },
                { "hr", "c1ee3fa862a58df1a40e2e903b86dd3f871ea7c0f68a8f4eb069add3840c2c2ab28b3f7631c7a4db597bbcb73dc82192aa8e5217f50062553afb47d652d99758" },
                { "hsb", "f6db595088534662440328f958d92693081c257c3fa83a79462585a69bdd51e3f502808b5ed42b400491468e8556c27ab55a09f137aa95301971a4daabbc9f57" },
                { "hu", "f78d92b406bedb94bef98020af140a6eabb0f6874db43537850aaf736ce527eff4c496f43736fc86cf1eb309d9a7dcba990a8bfadaf5b52e396347fe8d4ec255" },
                { "hy-AM", "8fd2931c39ea495dd07d0386c5d13c4bfa676f2226ceb15190b6737198b31f853b34769432195c173c20dcdf46195349558e80a6e684ba5af612f76652db8cc7" },
                { "id", "a6e666faac296901a6254258edf0f2590c7a840ed50f910265f6822fdc6d2da1027484fd2557ca1e502a98e8697e1c5f47662ac7a166c5e74174af38b75f3ef6" },
                { "is", "9987d491c7de0461e264837d1097ade26b782dd189a2d8737f89297ebcadb925e31657073af6e4504099c02073fcabf3812e182bbd395c32a2bc2d25fa07e995" },
                { "it", "4682caecf646947f5db89c6b1968b3326c67e6cb4a2b296b4bd06de22e69e2e1fdfb7ec868c3dd198967db0c6bfa98ee2ebb159dce2ea5f7f6c4a470398f1072" },
                { "ja", "641658a84759bdc280c28e9d4460203119a12949c0629da22e4c60e29af627351b0678d48f6c4080440e5eaf33817094a88509779e2153c1437ca62ed5dd4f80" },
                { "ka", "91483d1ba052a43afe85000b25d523bbd5533ea27711d9af90f0ef821a7fbe2579319b72faad55e4eaa9610ce756e1f16a81e75e4f569c847c0667b84a2babe8" },
                { "kab", "30bf5ef5aa4cb1f82eefb64197f7c16cc3f7d49a97e96ce9da076779aa7c872f1f15862f8320c0f774a26922704b836c59aaf838f62e8226937bcda15cfc0944" },
                { "kk", "5062a2636fee903af51bb39096f3a2702626f3ceb9f9ec14964105a928625074853d5e11a00e087532269bab864858509be885f3dab205cfed163b90ffa3cc5b" },
                { "ko", "0a94b6e0e4c0f00eaa879935011ae581604330dc91a6a7868b5ec664f29ed36c532aa9ff6dbd5337a2254d728369f754e1ddf87f2ad47b6734db0942fbca033e" },
                { "lt", "94e3fa6457e31e73c72fb927dc7a1c74239eec1ef90fab1950f43c01c1801e569452fe22dcec665d1de295d02ecc2d59834af98d918c21aa90447b107259dda6" },
                { "lv", "e9262ac61f39679abf51fbad3ed87f399ee7455cd441a5ea161e2b01d9f3663c8355d7a2d067ad8d7333edbcd2275a71cf37fce7599787bd07eacdd319b942cf" },
                { "ms", "17fae871b0f05fc318c4bd1de39a667c33eabd8a4486547358220eb948fd0a6c6e5484fdee166958fd1c83bb6e7d4eaedfd650979c49751cac6ea5bf5ba0415d" },
                { "nb-NO", "d1cc1b05d24585a81c5e52d8b5633f5f650e3412355f4a459bc7c9c49892404d8ff7f8aeff9b4588f9818a2e3d974853cada397694aaaa46ec284e4b3ba841ce" },
                { "nl", "4a7d8d180895e9cc85757922172c0cdc7c10cf75bb52f543b088e9210df65666d06db2c9c514c8e5e420f867d9e5b24377d55d1fd06f39647535f7e109fdad59" },
                { "nn-NO", "20b1e5128c4a7d6dd91911f2d9fe30613f96cc5aef032f49f59090b1a5dd65a27a20e1a61b28f291161c19300fd7c8b9150b65b94c55f663eb52a0fee74bda10" },
                { "pa-IN", "ec2badba1643515ca4e53d5740f61f54abc342d83954a71912ba9f1792121b23109ec3790a0e842d85b47abc02c71454f0fb4070bced5518497d85b0ad49da97" },
                { "pl", "540e4a61e3316bc6bed64ed75741649740446d63a3364944bf2d66e413fa817f851631a97d27cc4f5a4730f01b79441cc3f0044a5ad51af6d74aecb23a8c4f9d" },
                { "pt-BR", "4f86afbbdee71e77f25db2db955ce81f44b572bde5480d8f3973a3ee9b0c99a26feef0bcbcc3296676abd1d08d76090e01b79100bd4608508f266f9f899b226c" },
                { "pt-PT", "6e6d0b205bc850f0a93eb2ce9a3fd0ba7c9da2b2ad6faa43f726d36586b501d23ba4ae47671fc6a45b752a98d6d6d0d5489e0fe1b31c897057b22048e1fd58b6" },
                { "rm", "d66624f8b145e9815ee2844bee796bff59829a6f8bdbcdddb356f9000b1fd0563cf96d05ad24f419a3f511b06d5d95da7eb133005d649f3ee8998704c67ffc5a" },
                { "ro", "173fe040f8b5492cc083131278e82c3d290c96717ac87b54f2bca86f4915a92dcbf5dae40510cf1ce00640b913bff2444253e6d6a75519e2a5601316d7b9e3cf" },
                { "ru", "6d56dcf73101b219216bd76b8fd02ae89c0f2a9f14201d4152a698ebff593585a727d4a706d8f0097583683ea4aca97b0fa7e862e6ae2fcace7445adb3852085" },
                { "sk", "efdb704a6387cd8b0f0eb5a1c17749fc8fb883bd4baa5166d3225d32bc34254a0e371d146b7a6827161aea2d4cc99663f8e550bf4670245770a887d76568886c" },
                { "sl", "7da482f1f8118d72cb1cf94865b2e5b8ddbe1afc164eda9277f815befe1d8fbc0a3c676446f6fda3307d4abed5e434cd46ba8ecc66712f8211ed5e4e976ea199" },
                { "sq", "14c1e172c252c7f2b2690b5b3fd5ec8757d879f16ec81b5c8ac2e50f69034f713a8029f890fb21f7312fd45b5ee9d1d6d7c033d3f71cd334f3bd31529caba360" },
                { "sr", "4160f14a96c18b1eb94564a3acad8a5fdc1e7c487edf4a47184c8f7fb3cd27ac284e25ca77310bffbb6e268b1e4eb83940f796afbdd9b268c14e721da76481c9" },
                { "sv-SE", "b27afb1ee262e243494ba692e766fa202a134dd32cfe218e485632ec1bba233cf00d9aca72c1f8cb1e2386d15a0d0e7d600917b2958098896c1612bd445bbd1a" },
                { "th", "7cdcabeceea0d5a394bd9bcf5298fe6262e64718cc50f2973d986d606b1eb021a7cd54a11d22116c848fc56e92b02eaca0c4074b380fc0ea68d2d92899900897" },
                { "tr", "7356dc27c9404201ea1fbead697dbf50bbad485811da8afb815af3796da280fb300322d82dc3775126462a6402f36887889ee899814a860a5e4438f018f69470" },
                { "uk", "f0b0b5d65fbc66f1afdc983aecf0611681a4781a07e31de887a73343d8beb0b2bcf1215a944950fe93e34bad1c2fea54a858e15ec5d9fe010b9f846576a51138" },
                { "uz", "ebd9dd57415de02ac09a63cf20eb421b6324c1dbb306976ac161746337540c3c8c0114ba7f547b1e60fbf8cc2a93f56d5ee2879492b3aa9cef5c9257e95f1318" },
                { "vi", "a1fcc11ea5636c9667584e6fcaa6e2ea0bfe217c96b6ee28034db2be5434067dad76fbbd987aa553799581d36f964d7c478931f49f1d4d7eda77bc4bd1a61a8a" },
                { "zh-CN", "743752be988fe3db9a9635f5b4ad2f3d2f5237b551831a470dc5e64c0f0ea60cc49cd55a59030612f5d2d2cbe7af4797611f90d79fd3fd83bb84618165d9be60" },
                { "zh-TW", "35fa02f025ed9f4a207666191c37a2b638ca2903900d66bde1ec5392d0d12c308a9ded7b433672fc4fe3d5e2bedab84e134df9882ab64a5a7488c65103f2b43c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.2.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "61203d69029b6b54fba1b574832c2507d7ad49e95b2e4a88236b1f36215d12c950f1981ab6b9ff9fbf13b9fc2d1314802291349e9c4fdab184b7165534fc5fa5" },
                { "ar", "e85a6ffa59d626e1069ce069a5be26a274d0c286f73958fc114cd0e921963d8dc85403492b763e99a54feef8d191d5a6fec79ff6e26d416637994edee17dd0cb" },
                { "ast", "dc89c1b721394af7f7603b648e27ba670189a519fadcea658dd980eade29e345b00bc8f432af08861c5f7b4f3c4e383a67564f73057b06e1f3787461de16fcd4" },
                { "be", "86792f485d7454d6847b74c76923b9ab69c3b936a9c3df5bf245fda20731f43235c0675ba999ddbc12cea0095a87655b0a1f707bb42793092352c4e09a0f17cf" },
                { "bg", "fad6eff0eadd85bc905a8875e83c5645effd09590f0f4d1fe687bffe7968f49b629ba94f9a02c98d67de5ffb583f6be3765fc6a5cddca4ecbdd6332de0d9c14e" },
                { "br", "eab66c60816064cc33a119e7a9b08a48b09c93561e5e587ee21e61adbf92c85e6c7aa4b1a45484d8a016dd5a10b5fceebfc51191564e1dae90735533aff02465" },
                { "ca", "e4a0693adb558699e03ecaeeedb1f189af61a62150d72afd089b4b04c55dbd943a2c4bf51950d6f4cc2883c6cce647a7178c10eda0dcedc92aad272b60792349" },
                { "cak", "20c76c599819a701bd9f9560c3547cc988a5599f7436c6a8ad1a92b40946be94b128ebca33eab3fd812e4b8a2a09d0caa8c2d3f4209c528e55f33c0b811118ce" },
                { "cs", "99b36fff90c94620cd15f6c5deec5a2f8660d16b188a1ea34900b1a4682b80a50b2a0e420d1e8bcd93afb17bb26a63a1958824151fc331f93a76a7e3b60609c1" },
                { "cy", "759df791476bb283cb699b963fc40c0669414c1cdb0b768a3747bc1479dbba997b28d45d8f3e18855f4184d84f112c6e80afbd2512e7efddccef4bb14d1c4bc0" },
                { "da", "60f7a6536b9887e673cfb87d2ae8a2a68efd2d8d7a433e44d85be63a90e90f28cdc447266a48ae38c46a6ca10e89a763b7848dd664f18617e48cb3e9a0bacb34" },
                { "de", "e1c68c06622367d47f78cb1cabfc41e74880bd06c2c79819905e69818217347f44526a4e302c837f1b6dacc0365f18fbb72e5ae7ad662ef823a2357d102532b7" },
                { "dsb", "31714170e614435e8bff2c3f49d12e6ccfba0cec8910f200e66d305670b5828e316683323c4ac17843b476286ff7616cb51b85dcbb5c48c623e211283d224f01" },
                { "el", "44b80c9c64d0a133d21af41ad6b4af70b3dd0cd7f8cc519d810627902b9f3ef93e3c2a8126b3db70a23354406aa78409d2e3b2c8b0da913bf55c5d42399a0cc8" },
                { "en-CA", "12cb2f3684d5d30fd24d2cf2e01160845120098b95816751f95a9699283fb52309f4aa96e0c9978d69cb94ccaf101437e501525382960993771bec84f3bb8c54" },
                { "en-GB", "5f64a16ac3c89df884c0da9c5324734b5db89aa769e9601a8a5efa7044c51a52311ee8cda2d71dfa996215f49f2843499bdd368d86aee449a35682cc330d1f33" },
                { "en-US", "655a43eeab782407e9639955070c758873b5604251cda406802661ae5c411b9d223b92f725674745af7e83ced17fc37b54da51b153023673ddde49d32bb0fd11" },
                { "es-AR", "58b64c2da5f45d4256612b26909775f6df3e7a13d71626a0e2fe28194d3cad5c6761a7418fb88052922025b5cddce7680600681bb752e68679ef73640ccdcfe3" },
                { "es-ES", "c08a79a73675fbb3535f6313948fe0ffc3c1ea0f629c948803136b231216692200352d5f55843a1f58e16d7a6adf845875005c62ff327337d0a00013ff337c75" },
                { "es-MX", "266fa813127f0aa737c75339410a9a931add7f1c96978a1de19ef9ce283bd1de26799cee500ef5700537962741da77d0d4f4e0da5e020459173fce957fb964cc" },
                { "et", "259915c32757455278ae85e60406469c0aed6b3aba53fc628aee1d1a5c417ffd048d3e2091907db7f4ddb683967780e0dd94da84ee339b87624a2ac112679d0f" },
                { "eu", "6854d0c2ddc3f36a3addffbf9330302ccb9617ead172d7a85762b09422efc257902ff1220a68f77052951e4d9c53c2e40dcc0773fbe3385499b051a52a56a520" },
                { "fi", "5a064c5de37993beaf88ac909530e4367d6f074562d768c7c3e2397134a077c75b5cd86a084c2c66889bcf0821d251431974ffa370b8845e9e94efa70c55b300" },
                { "fr", "4a44c135163791a7536d58c1f50c34a7f39001c0492e75f9078f26cd235a18d695f39d7acbda167f1771c1df204d17c1863ef2d712c1ecd75712f90bc49191fe" },
                { "fy-NL", "15d9d62b2234c35bcc5f99234c8f16aa6c743bdad549812822a0e6bfd46a8c72bd4bfce7889a9893d7d382610506bf8fdd3e3f67b02f069200422a65a99285bb" },
                { "ga-IE", "1043bc4e7589b7a2638563d9a6790ea30b6fb5e25e1f049b70865eab433e6e089289f7483d73cf5dd6acdca1f9287a47ec56761f44370d5fc90b70d13c34b960" },
                { "gd", "0402518d8c84e99355b05b0c6df6fcc83d724b109e1dfe540038ef02f43b0db3e3efb60630022e83b5268a47dc8127da970e5b1df0c7db07db222ca4b008f7f2" },
                { "gl", "a356ba508eee388c4f6d99021ad2f9d9e2a819707b1c49d8625a5da3a9f896d22101fcc1ae49f08844d24719aa322483341c62dd28ca0aee5284524814a1c65b" },
                { "he", "20f5c79ce353cfea38e3d2f642e2f3ec3caaa1434f037814c6c85d23c8c96c72b5b765adf422066583055b3c079aa03b352c76b501dde982704af14adaa77f6c" },
                { "hr", "207ed883298f4daaddbbcabcaf077f2938e062add2404d6f2c562d193102f5737e03999d53de2c562d36ad22732ce40010177d6df6e2cd1306935190163bc1b2" },
                { "hsb", "b06ca15089ccb270e216830bf41bf017829d66b017f107f490c95cd5e551f436ef7d4e01dc31f33faa8fbcb47edc001bd4ec192619e50fcd732b7f240df00476" },
                { "hu", "02a35228c4751c3a5bd9c637f46433d6644bd0c54e0f9c1740a85fd305cdef5231b31760fdc0235318bbdd533ca96bed073b0e140136d320382c3a78d47c3cee" },
                { "hy-AM", "3899f0ec521246dc22064bbdf1636f2420ea343c57a99821cd74dcd4d27ba1f767e8baf2dd47a7b3ab88d7041c3270f5ec797b1d63c00a0b6f43f60a375a2745" },
                { "id", "67c5604dd205cd91450e5fcf29294216d13743cf9228d2ca0cfb7fbb6801d2535109ae33befdee02f3968fb48ee0e85e6f7a2bc8bcf1bd15fc19e4ff1e650597" },
                { "is", "cb21864aa0d8deb3dbe9b1154dad374615a76f65eef13db86dc419b06646acc7a9cbb8d2fcac041a19ba0151d194fafefb056874315abdf86e24576020301712" },
                { "it", "f3192e47988cb04b8de9d025448b4ab9a7cdb971ae3ad9f06c8c49aec693dcf13e3ea4191ca8be6ba95c1b7e78402981b11b31e063cf9dd5aea023013d150257" },
                { "ja", "9e837960985d47fc8940942e5a123701afab663a7a2af3458c9cd12634e9e4fea5519a30948fe4c72f166f7b8645464c4692170032f5d4f72f28a3aaff2115f1" },
                { "ka", "0f0f753b000fb433a5ebb86c619e53221afe77a1592b93b5b175f28d5d53acaa033ca572a87f76274d1db730dfc7890a43f118c221a65e08c8b2ea31519289e6" },
                { "kab", "d67e3981be3257860a472b1654e1236295dbf2707aae557080da630d5d0db64c82129e0e8d8cb5b2b5f7790ceeee78f6d95797aa5b83b1ecfa91c055fe8ede65" },
                { "kk", "8315fd8634281910f82d82d7b4cdfabcc5b9c149558c4626ecbe9bd66d81bf2acf3bacb004ffd9355681e41a8bb83f3566bab7c601b969773ec725cdd1823279" },
                { "ko", "50737dbef0e215dac0942f6a6872b36d9010511023751c77ea22435fe73a2d40c71e5c39967ce85d6b4f50868e549cf033619e5a4a340ca277fb5e86d90c28fe" },
                { "lt", "ab8bafdc17cc57bec2a36e2e02ef8febdd0106959569d8d66c90a6e606e9fd8aa7cb199cfab77d15923ad4a10c8177391a62b48278e6d3dbf61a16dc795a8191" },
                { "lv", "0073218d32b677dde98fc43580bcc1b80f578cb2fd224439d4fbe625c2dbbf1ff83325f755bbf43dd4905e3621086e512bef177c1886ac057f473844c0123346" },
                { "ms", "f82bdd3151061bb80cbbbc98925d0cefe3bf4038dba542d4c6da1f71d8ca2636d591eff4c308921e80f2ba3748dd4dbe4093b2876a7b088a210ce4603243e2f6" },
                { "nb-NO", "96b819450f4f08aca8e396f417c0009ba213824206a97ab4892fe754d0371c46ee156a6a69f053b3a7ffb55d9bad238a5cb5655e1c6839f3cca3da41f04a42a4" },
                { "nl", "553afa3149511009b2e3391a2a6c01053b4e8e86bed476b4cd2349f4d8926ea6d9a2caca4326f5e34b19df6aae39afb6376599856fd2b21dd9ee45100d1db270" },
                { "nn-NO", "75bea5cdecb2459efcff0ad5e8fc6f09664db622c777021acc98e3aa44638bfd8aac4470bce4a34fe8be0797d814e8e6265acf1be77ca689a588dac4d46d5c8e" },
                { "pa-IN", "8623d46abfea8ac2dd0a9ffa457d4b1a6429e4b260bae3bd3a8a2df308e10830c0d852e3a4cb4c27932173a77606bde99a547e8fba494fba4f1733c54383e621" },
                { "pl", "2182db2ca0024f868a3cc5345fdfa1b6b45ec7ab77a3abeccd18e4fbafebe4f451a8c304194370beb3bd0813bf92a32eea874da4506b577d59a20d0012a81479" },
                { "pt-BR", "8d5f5ee45c25a98d24d7a0e9279c456a622d5179066dba7d86d3b191563f2a77b6c24bdaae24b55461d920b7425ca658723de33f2e395a97c1a76e24189da7a8" },
                { "pt-PT", "321e8a1e4a4be6d9cf211b228679f4024579990a3fa6b6844594ca6756c2af2905dc3e5b020f66d75f9162463ec7dbca2f1550c2b45d86f927ec1cc135cbc2bd" },
                { "rm", "328c21990aeb9568445cb3477c2891017bd6db7ad02ebb1fd614dac962ceb26b3987dce28da8927e6a0b40c0dcd5de6a7081a6710c77a1eecf7b7d6d178e5a53" },
                { "ro", "b76b9e4a51e2032480f1a7f72dffead3439757f42c39c29dda208e64c2a9d07c3b06485a327173155f3d6cfbef33956d322c1564f0bdf155c3c1007a92d5ce50" },
                { "ru", "7964e95d4594d16c62dd2c95ebf00f7b02de5eab0c73f00b38c36c598ea5bdead9ce58dd64d55c2d95c0a904a38db1469a60620c6a02f54cfc8db9909c8613dc" },
                { "sk", "da2a52b554c4bf15488d419d3d18c6d1b8c4751a433d4c9dd17033bd673e4ba835003e41934ffc44e9e8d9a4119046c49334899a103b1dff71a755f46d9e54c8" },
                { "sl", "439e325c7ff688380e1c05e99d7b9eddc34adb20d8c0cd001fbe9e8bbb6e9b9aea7a125328b1cc3190d1ea7f17c8bfbb99ba1f2da9b1b46a4e4a1c3e5bea181c" },
                { "sq", "7490a6e604c59575ef949f1c48dbcf0aec6938b9067ac158dbfd58ed28403459d8a78586214e89fd11a0ec786dab101eafc0724fd524ca6e71bb6355d1913478" },
                { "sr", "1c1ebfedcf8e1dbdf414d85274fe0c5c399fcc04808cf2bb755fa91f377ecf596aadcb182957f9e061338a368c3ceddfd08aa0660bb98249d9faf90abc8772fc" },
                { "sv-SE", "77365c6eb3409a3b06a6478ce675f026cbd5f2d91218362802e62a736319c47dc7279726e63a0bd1547b039ea223a26253f7f63f6c1657111e4b0e1516d612fe" },
                { "th", "0fdc38642680d7bc3f75856292dc4c152d1d061dc229912199d3fb0ff4dcaece327b6275ad43726d898ac0153e415d7e7b3496e1976f548e15fe58e6729b44c5" },
                { "tr", "5f666136f5dc7e16bd4991fe420d609c1d42e95b0cb2f87db24d55184109a93bd263f136f7d9bc88d14022da1a1a7f1300d940ad9b623a619c3f3e82715b3100" },
                { "uk", "cf96a4f06c681a05c12d2bb9709cd0390e4e0eda0e3fbbd76182faac228cb1fb6c418d800c53e6973e3b3cfddebdb86e344a511d5df48e1bedafd99fd929464c" },
                { "uz", "3b9dffa0f6b83eed281a8b128a0cf35407df452cff5003078c9b0d69ff87daebf0de269d22663ae7345ccf92c887377baf95a8c45b2187829cebf6c36efdd1c3" },
                { "vi", "f222775dfc857d40e9a75fd6ea164ef09a56b6949df2a10eb5d6f8de7ecceebd31ccf9cb43c0046a243149f0150d6766e2a9c61cbcdff7d2963655c98ca7f087" },
                { "zh-CN", "0e01bf930259248fde9d567b0baa7b65dab4e7586e48c295534cc73c1842df5ff83c3198640e7fb0456f4e7e0474cef5ac4cb84a3382f2316347b424419db65d" },
                { "zh-TW", "08d2cc8f4a60855d09f797f4d8387b356b5c5878a0439e6e373d693af8a3f652579b7fc35a655d9c49c06929ca5281453508728e289d592c09bf7501e8365d60" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.2.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new HttpClient())
            {
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
